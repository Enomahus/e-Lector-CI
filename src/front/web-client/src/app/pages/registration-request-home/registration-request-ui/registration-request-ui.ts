import { DatePipe } from '@angular/common';
import {
  Component,
  computed,
  DestroyRef,
  effect,
  inject,
  input,
  OnChanges,
  OnInit,
  output,
  signal,
  SimpleChanges,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { ConstituencyNode } from '@app/models/constituency.model';
import {
  allGenders,
  allMaritalStatus,
  allPersonTitle,
  allRegistrationDocumentType,
  allRegistrationRequestType,
} from '@app/pages/types/enumerations';
import { CitizenApiService } from '@app/services/api/citizen.api.service';
import { ConstituencyApiService } from '@app/services/api/constituency.api.service';
import { AuthService } from '@app/services/auth/auth.service';
import { PermissionDirective } from '@app/services/auth/permission.directive';
import { BreadcrumbService } from '@app/services/breadcrumb.service';
import { ConstituencyTreeHelperService } from '@app/services/constituency-tree-helper.service';
import {
  Gender,
  GetCitizensResponse,
  GetConstituenciesResponse,
  GetRegistrationRequestResponse,
  RegistrationRequestModel,
  RegistrationStatus,
} from '@app/services/nswag/api-nswag-client'; //'@app/services/nswag/api-nswag-client';
import { InputDatepickerUi } from '@app/shared/input-datepicker-ui/input-datepicker-ui';
import { Loader } from '@app/shared/loader/loader';
import { ParentModalUi } from '@app/shared/parent-modal-ui/parent-modal-ui';
import { StickyButtonsContainer } from '@app/shared/sticky-buttons-container/sticky-buttons-container';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { forkJoin } from 'rxjs';
import {
  CitizenForm,
  createRegistrationRequestForm,
  RegistrationRequestForm,
  RequestDocumentsForm,
  RequestsForm,
  ResidenceForm,
} from './registration-request-form';

@Component({
  selector: 'app-registration-request-ui',
  imports: [
    TranslateModule,
    FormsModule,
    ReactiveFormsModule,
    StickyButtonsContainer,
    Loader,
    PermissionDirective,
    InputDatepickerUi,
  ],
  providers: [DatePipe],
  templateUrl: './registration-request-ui.html',
  styleUrls: ['./registration-request-ui.scss'],
})
export class RegistrationRequestUi implements OnInit, OnChanges {
  isCreateMode = input<boolean>(false);
  isSaving = input<boolean>(false);
  constituencyId = input<number | undefined>(undefined);
  registrationRequest = input<GetRegistrationRequestResponse | undefined>(undefined);
  selectedStatus = input<RegistrationStatus | null>(null);
  saveTriggered = output<{
    registrationRequest: RegistrationRequestModel;
    certificateOfNationalityAttachments?: File;
    cniAttachments?: File;
    photoAttachments?: File;
  }>();
  //approvedTriggered = output<RegistrationRequestForm>();
  approvedTriggered = output<{
    newStatus: RegistrationStatus;
    requestId: string;
    reason: string;
  }>();
  viewDocumentTriggered = output<string>();
  goBack = output<void>();

  private readonly breadcrumbService = inject(BreadcrumbService);
  private readonly translateService = inject(TranslateService);
  private readonly authService = inject(AuthService);
  private readonly store = inject(ConstituencyTreeHelperService);
  private readonly citizenService = inject(CitizenApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly dialog = inject(MatDialog);
  private readonly datePipe = inject(DatePipe);
  private readonly constituencieService = inject(ConstituencyApiService);
  private readonly destroyRef = inject(DestroyRef);

  isLoading = signal<boolean>(false);
  isEditMode = signal<boolean>(false);
  form = signal<RegistrationRequestForm>(createRegistrationRequestForm());
  registrationRequestId = signal<string | undefined>(undefined);
  municipalityId = signal<number | null>(null);
  constituencies = signal<GetConstituenciesResponse[] | null>(null);

  nodes = this.store.nodesData;
  selectedNode = this.store.selectedNode;
  initialTreeSelectedId = computed(() =>
    !this.isCreateMode() ? this.constituencyId() : undefined,
  );

  constituencySelected = signal<ConstituencyNode[]>([]);
  parents = signal<GetCitizensResponse[]>([]);

  // Signaux pour gérer l'état d'affichage strict des inputs
  protected fatherInputValue = signal<string>('');
  protected motherInputValue = signal<string>('');

  // Signaux pour suivre l'état des documents déjà enregistrés en base
  existingCertificateName = signal<string | null>(null);
  existingPhotoName = signal<string | null>(null);
  // Conserver les IDs des documents pour la visualisation
  existingCertificateId = signal<string | null>(null);
  existingPhotoId = signal<string | null>(null);

  // Computed properties pour filtrer les datalists
  protected filteredFatherOptions = computed(() => this.filterParents(this.fatherInputValue()));
  protected filteredMotherOptions = computed(() => this.filterParents(this.motherInputValue()));

  allRegistrationRequestType = allRegistrationRequestType;
  allMaritalStatus = allMaritalStatus;
  allGenders = allGenders;
  allPersonTitle = allPersonTitle;
  allRegistrationDocumentType = allRegistrationDocumentType;

  certificateFile = signal<File | null>(null);
  cniFile = signal<File | null>(null);
  photoFile = signal<File | null>(null);
  isSubmitting = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  allRegion = signal<GetConstituenciesResponse[] | null>(null);
  selectedRegionId = signal<number | null>(null);
  selectedDepartmentId = signal<number | null>(null);
  selectedSubPrefectureId = signal<number | null>(null);
  selectedMunicipalityId = signal<number | null>(null);

  allDepartement = computed<GetConstituenciesResponse[]>(() => {
    const regionId = this.selectedRegionId();
    if (!regionId) return [];

    const selectedRegion = this.allRegion()?.find((r) => r.id === regionId);
    return selectedRegion?.children ?? [];
  });

  allSubPrefectures = computed<GetConstituenciesResponse[]>(() => {
    const departmentId = this.selectedDepartmentId();
    if (!departmentId) return [];

    const selectedDepartment = this.allDepartement()?.find((d) => d.id === departmentId);
    return selectedDepartment?.children ?? [];
  });

  allMunicipalities = computed<GetConstituenciesResponse[]>(() => {
    const subPrefectureId = this.selectedSubPrefectureId();
    if (!subPrefectureId) return [];

    const selectedSubPrefecture = this.allSubPrefectures()?.find((sp) => sp.id === subPrefectureId);
    return selectedSubPrefecture?.children ?? [];
  });

  allVottingLocation = computed<GetConstituenciesResponse[]>(() => {
    const municipalityId = this.selectedMunicipalityId();
    if (!municipalityId) return [];

    const selectedMunicipality = this.allMunicipalities()?.find((vl) => vl.id === municipalityId);
    return selectedMunicipality?.children ?? [];
  });

  requestForm(): RequestsForm {
    return this.form().controls.request;
  }
  citizenForm(): CitizenForm {
    return this.form().controls.citizen;
  }
  requestDocumentsForm(): RequestDocumentsForm {
    return this.form().controls.requestDocuments;
  }
  residenceForm(): ResidenceForm {
    return this.form().controls.residence;
  }

  constructor() {
    effect(() => {
      const nodes = this.nodes();
      const id = this.constituencyId();

      if (nodes.length > 0 && id) {
        this.store.expandNodePath(id);
        this.store.setSelectedNode(id);
      }
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['registrationRequest'] && this.registrationRequest()) {
      this.form.update(() => createRegistrationRequestForm());
      this.isLoading.set(true);
      this.loadRegistrationRequest();
      this.isLoading.set(false);
    }
  }

  ngOnInit(): void {
    const idParam = this.route.snapshot.params['id'];
    if (idParam) {
      this.registrationRequestId.set(idParam as string);
      this.isEditMode.set(true);
    }

    const getConstituenciesData = this.constituencieService.getConstituencyTree({});
    const getCitizensData = this.citizenService.getCitizens();

    forkJoin([getConstituenciesData, getCitizensData]).subscribe({
      next: ([constituenciesData, citizensData]) => {
        this.parents.set(citizensData.data ?? []);
        this.syncInputValues();

        this.constituencies.set(constituenciesData.data!);
        const regionDataLevel = this.constituencies()?.filter((d) => d.level === 'region');
        this.allRegion.set(regionDataLevel!);

        if (this.isEditMode() && this.registrationRequest()?.constituencyId) {
          const munId = this.registrationRequest()?.constituencyId;
          this.getResidenceData(munId!);
        } else {
          const controls = this.residenceForm().controls;
          if (controls.regionId.value) this.selectedRegionId.set(Number(controls.regionId.value));
          if (controls.departmentId.value)
            this.selectedDepartmentId.set(Number(controls.departmentId.value));
          if (controls.subPrefectureId.value)
            this.selectedSubPrefectureId.set(Number(controls.subPrefectureId.value));
          if (controls.vottingLocationId.value)
            this.selectedMunicipalityId.set(Number(controls.municipalityId.value));
          this.toggleControlStates();
        }
      },
    });
    this.setupFormLinkage();
    this.setBreadcrumbs();
  }

  private filterParents(query: string): GetCitizensResponse[] {
    const q = query.toLowerCase().trim();
    if (!q) return this.parents();
    return this.parents().filter(
      (p) => p.firstName?.toLowerCase().includes(q) || p.lastName?.toLowerCase().includes(q),
    );
  }

  protected getParentDisplayName(parent: GetCitizensResponse): string {
    const date = parent.birthDate ? this.datePipe.transform(parent.birthDate, 'dd/MM/yyyy') : 'N/A';
    return `${parent.firstName} ${parent.lastName?.toUpperCase()} (${date}) ${parent.birthPlace}`.trim();
  }

  private setupFormLinkage(): void {
    const formControls = this.residenceForm().controls;

    // Écoute de la Région
    formControls.regionId.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((value) => {
        this.selectedRegionId.set(value ? Number(value) : null);

        // Reset en cascade des valeurs enfants sans propager d'événements de boucle
        formControls.departmentId.setValue(undefined, { emitEvent: false });
        formControls.subPrefectureId.setValue(undefined, { emitEvent: false });
        formControls.municipalityId.setValue(undefined, { emitEvent: false });

        this.selectedDepartmentId.set(null);
        this.selectedSubPrefectureId.set(null);

        this.toggleControlStates();
      });

    // Écoute du Département
    formControls.departmentId.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((value) => {
        this.selectedDepartmentId.set(value ? Number(value) : null);

        formControls.subPrefectureId.setValue(undefined, { emitEvent: false });
        formControls.municipalityId.setValue(undefined, { emitEvent: false });

        this.selectedSubPrefectureId.set(null);

        this.toggleControlStates();
      });

    // Écoute de la Sous-Préfecture
    formControls.subPrefectureId.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((value) => {
        this.selectedSubPrefectureId.set(value ? Number(value) : null);
        formControls.municipalityId.setValue(undefined, { emitEvent: false });

        this.toggleControlStates();
      });

    // 3. Écoute spécifique de la Municipalité pour émettre vers le parent
    formControls.municipalityId.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((value) => {
        const municipalityId = value ? Number(value) : null;
        const constituency = this.store.findNode(municipalityId!);
        this.constituencySelected.set([constituency!]);

        this.form().controls.request.controls.constituencyId.setValue(municipalityId!);
      });
  }

  private toggleControlStates(): void {
    const controls = this.residenceForm().controls;

    // 1. Département actif ssi Région renseignée
    if (controls.regionId.value) {
      controls.departmentId.enable({ emitEvent: false });
    } else {
      controls.departmentId.disable({ emitEvent: false });
    }

    // 2. Sous-Préfecture active ssi Département renseigné et actif
    if (controls.departmentId.value && controls.departmentId.enabled) {
      controls.subPrefectureId.enable({ emitEvent: false });
    } else {
      controls.subPrefectureId.disable({ emitEvent: false });
    }

    // 3. Municipalité active ssi Sous-Préfecture renseignée et active
    if (controls.subPrefectureId.value && controls.subPrefectureId.enabled) {
      controls.municipalityId.enable({ emitEvent: false });
    } else {
      controls.municipalityId.disable({ emitEvent: false });
    }

    // 4. Lieu de vôte active ssi Municipalité renseigné et active
    if (controls.municipalityId.value && controls.municipalityId.enabled) {
      controls.vottingLocationId.enable({ emitEvent: false });
    } else {
      controls.vottingLocationId.disable({ emitEvent: false });
    }
  }

  onNodeSelected(node: ConstituencyNode): void {
    this.store.setSelectedNode(node);
    this.constituencySelected.set([node]);
    this.form().patchValue({ request: { constituencyId: node.id } });
    this.form().controls.request.get('constituencyId')?.markAsDirty();
  }

  private async loadRegistrationRequest(): Promise<void> {
    const requestData = this.registrationRequest();
    if (!this.isCreateMode() && requestData) {
      this.isEditMode.set(true);
      const documentData = requestData.documents?.find(
        (d) =>
          d.registrationRequestId === requestData.id &&
          d.documentType === 'identityDocumentOrNationalCertificate',
      );
      this.form().patchValue(
        {
          id: requestData.id?.toString(),
          request: {
            constituencyId: requestData.constituencyId,
            registrationType: requestData.registrationRequestType,
            reasonForRejection: requestData.reasonForRejection,
          },
          citizen: {
            gender: requestData.citizen?.gender,
            firstName: requestData.citizen?.firstName,
            lastName: requestData.citizen?.lastName,
            birthDate: requestData.citizen?.birthDate
              ? new Date(requestData.citizen?.birthDate!)
              : undefined,
            birthPlace: requestData.citizen?.birthPlace,
            maritalStatus: requestData.citizen?.maritalStatus,
            marriedName: requestData.citizen?.marriedName,
            nationality: requestData.citizen?.nationality,
            profession: requestData.citizen?.profession,
            email: requestData.citizen?.email,
            physicalAddress: requestData.citizen?.physicalAddress,
            postalAddress: requestData.citizen?.postalAddress,
            fatherId: requestData.citizen?.fatherId,
            motherId: requestData.citizen?.motherId,
          },
          requestDocuments: {
            partNumber: documentData?.partNumber,
            issueDate: documentData?.issueDate ? new Date(documentData?.issueDate) : undefined,
            expiryDate: documentData?.expiryDate ? new Date(documentData?.expiryDate) : undefined,
            issuePlace: documentData?.issuePlace,
            registrationDocumentType: documentData?.documentType,
          },
        },
        { emitEvent: false },
      );

      if (
        requestData.identityDocumentOrCertificateIds &&
        requestData.identityDocumentOrCertificateIds.length > 0
      ) {
        this.existingCertificateName.set(
          `Document_Identité_${requestData.identityDocumentOrCertificateIds}.pdf`,
        );
        this.existingCertificateId.set(requestData.identityDocumentOrCertificateIds);
        this.requestDocumentsForm().controls.registrationCniOrCretificateAttachments.clearValidators();
        this.requestDocumentsForm().controls.registrationCniOrCretificateAttachments.updateValueAndValidity(
          { emitEvent: false },
        );
      }

      if (requestData.photoIds && requestData.photoIds.length > 0) {
        this.existingPhotoName.set(`Photo_Identite_${requestData.photoIds}.jpg`);
        this.existingPhotoId.set(requestData.photoIds);
        this.requestDocumentsForm().controls.photoAttachments.clearValidators();
        this.requestDocumentsForm().controls.photoAttachments.updateValueAndValidity({
          emitEvent: false,
        });
      }

      const constituency = this.store.findNode(requestData.constituencyId!);
      if (constituency) {
        this.constituencySelected.set([constituency]);
      }

      this.syncInputValues();

      const targetConstituencyId = requestData.constituencyId;
      if (targetConstituencyId && this.allRegion() && this.allRegion()!.length > 0) {
        this.getResidenceData(targetConstituencyId);
      }
    }
  }

  private getResidenceData(locationId: number): void {
    const regions = this.allRegion();
    if (!regions || regions.length === 0) return;

    const path = this.findConstituencyPath(regions, locationId);

    if (path && path.length > 0) {
      const regionId = path[0]?.id;
      const deptId = path[1]?.id;
      const subPrefId = path[2]?.id;
      const munId = path[3]?.id || path[path.length - 1]?.id;

      if (regionId) this.selectedRegionId.set(regionId);
      if (deptId) this.selectedDepartmentId.set(deptId);
      if (subPrefId) this.selectedSubPrefectureId.set(subPrefId);
      if (munId) this.selectedMunicipalityId.set(munId);

      this.residenceForm().patchValue(
        {
          regionId: regionId,
          departmentId: deptId,
          subPrefectureId: subPrefId,
          municipalityId: munId,
          vottingLocationId: locationId,
        },
        { emitEvent: false },
      );

      this.toggleControlStates();

      const constituency = this.store.findNode(locationId);
      if (constituency) {
        this.constituencySelected.set([constituency]);
      }
    }
  }

  private findConstituencyPath(
    nodes: GetConstituenciesResponse[],
    targetId: number,
    currentPath: GetConstituenciesResponse[] = [],
  ): GetConstituenciesResponse[] | null {
    for (const node of nodes) {
      const path = [...currentPath, node];
      if (node.id === targetId) {
        return path;
      }
      if (node.children && node.children.length > 0) {
        const found = this.findConstituencyPath(node.children, targetId, path);
        if (found) return found;
      }
    }
    return null;
  }

  async onSave(): Promise<void> {
    if (this.form().invalid) {
      this.form().markAllAsTouched();
      return;
    }
    const registrationRequest = this.getRegistrationRequestModel();
    this.saveTriggered.emit({
      registrationRequest,
      certificateOfNationalityAttachments: this.certificateFile() ?? undefined,
      cniAttachments: this.cniFile() ?? undefined,
      photoAttachments: this.photoFile() ?? undefined,
    });
  }

  onApprove(): void {
    const reason = this.form().controls.request.controls.reasonForRejection.value ?? '';
    const registrationRequestId = this.form().controls.id.value;
    const newStatus = this.form().controls.request.controls.status.value;

    // Sécurité : On s'assure que l'ID est présent avant d'émettre
    if (!registrationRequestId) {
      console.error("Impossible d'émettre l'approbation : l'ID de la demande est manquant.");
      return;
    }

    if (newStatus === undefined || newStatus === null) {
      console.error("Impossible d'émettre l'approbation : le nouveau statut est invalide.");
      return;
    }

    this.approvedTriggered.emit({
      newStatus: newStatus,
      requestId: registrationRequestId,
      reason: reason,
    });
  }

  onViewDocument(docId: string): void {
    if (docId) this.viewDocumentTriggered.emit(docId);
  }

  getRegistrationRequestModel(): RegistrationRequestModel {
    const formValue = this.form().getRawValue();
    const { id, request, citizen, requestDocuments } = formValue;

    return {
      id,
      constituencyId: request?.constituencyId,
      registrationRequestType: request?.registrationType,
      reasonForRejection: request?.reasonForRejection,

      // Le mapping du citoyen est maintenant beaucoup plus propre et direct
      citizen: citizen
        ? {
            ...citizen,
            birthDate: citizen.birthDate ? citizen.birthDate.toISOString() : undefined,
          }
        : undefined,

      // 3. Mapping correct du tableau de documents attendu par l'interface
      documents: requestDocuments
        ? [
            {
              documentType: requestDocuments.registrationDocumentType,
              partNumber: requestDocuments.partNumber,
              issuePlace: requestDocuments.issuePlace,
              issueDate: requestDocuments.issueDate?.toISOString(),
              expiryDate: requestDocuments.expiryDate?.toISOString(),
            },
          ]
        : undefined,
    };
  }

  cancel(): void {
    this.goBack.emit();
  }

  setBreadcrumbs(): void {
    this.authService.getPermissions().subscribe((permissions) => {
      let label = '';
      let targetRoute = '/home';
      if (permissions.includes('accessRegistrationRequestsForAdminPage')) {
        targetRoute = '/registration-requests-for-admin';
      } else if (permissions.includes('accessRegistrationRequestsForManagementPage')) {
        targetRoute = '/registration-requests-for-management';
      } else if (permissions.includes('accessUpdateRegistrationRequest')) {
        targetRoute = '/registration-requests';
      }

      if (permissions.some((p) => p === 'getRegistrationRequestForAdmin')) {
        label = this.translateService.instant('breadcrumb.registrationRequestsForAdmin');
      } else if (permissions.some((p) => p === 'getRegistrationRequestForManagement')) {
        label = this.translateService.instant('breadcrumb.registrationRequestsForManagement');
      } else if (permissions.some((p) => p === 'createRegistrationRequest')) {
        label = this.translateService.instant('breadcrumb.registrationRequests');
      }

      this.breadcrumbService.setBreadcrumbs([
        {
          label: label,
          url: targetRoute,
        },
        {
          label: this.isEditMode()
            ? (this.registrationRequest()?.reference ?? '')
            : this.translateService.instant('breadcrumb.registrationRequestAdd'),
        },
      ]);
    });
  }

  private syncInputValues(): void {
    const fId = this.citizenForm().controls.fatherId.value;
    if (fId) {
      const parent = this.parents().find((p) => p.id === fId);
      if (parent) this.fatherInputValue.set(this.getParentDisplayName(parent));
    }

    const mId = this.citizenForm().controls.motherId.value;
    if (mId) {
      const parent = this.parents().find((p) => p.id === mId);
      if (parent) this.motherInputValue.set(this.getParentDisplayName(parent));
    }
  }

  protected onParentSearchChange(event: Event, parentType: 'father' | 'mother'): void {
    const inputVal = (event.target as HTMLInputElement).value;
    const isFather = parentType === 'father';

    if (isFather) this.fatherInputValue.set(inputVal);
    else this.motherInputValue.set(inputVal);

    const matchedParent = this.parents().find((p) => this.getParentDisplayName(p) === inputVal);

    const control = this.citizenForm().get(`${parentType}Id`);
    if (control) {
      if (matchedParent) {
        control.setValue(matchedParent.id);
      } else {
        control.setValue(undefined);
      }
      control.markAsDirty();
    }
  }

  onFileSelectedTest(event: Event, type: 'cniOrcertificate' | 'photo'): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] || null;

    if (file && file.size === 0) {
      this.errorMessage.set(`Le fichier pour ${type} ne peut pas être vide.`);
      return;
    }

    this.errorMessage.set(null);

    switch (type) {
      case 'cniOrcertificate':
        this.certificateFile.set(file);
        this.requestDocumentsForm().controls.registrationCniOrCretificateAttachments.setValue(
          file ?? undefined,
        );
        this.requestDocumentsForm().controls.registrationCniOrCretificateAttachments.markAsDirty();
        break;
      case 'photo':
        this.photoFile.set(file);
        this.requestDocumentsForm().controls.photoAttachments.setValue(file ?? undefined);
        this.requestDocumentsForm().controls.photoAttachments.markAsDirty();
        break;
    }
  }

  protected openAddParentModal(gender: Gender): void {
    const dialogRef = this.dialog.open(ParentModalUi, {
      width: '470px',
      data: gender,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((newParent: GetCitizensResponse | undefined) => {
      if (newParent && newParent.id) {
        this.parents.update((currentParents) => [...currentParents, newParent]);

        const isFather = gender.toLowerCase() === 'm';
        const controlName = isFather ? 'fatherId' : 'motherId';
        const control = this.citizenForm().get(controlName);

        if (control) {
          control.setValue(newParent.id);
          control.markAsDirty();
          control.updateValueAndValidity();
        }

        if (isFather) {
          this.fatherInputValue.set(this.getParentDisplayName(newParent));
        } else {
          this.motherInputValue.set(this.getParentDisplayName(newParent));
        }
      }
    });
  }
}
