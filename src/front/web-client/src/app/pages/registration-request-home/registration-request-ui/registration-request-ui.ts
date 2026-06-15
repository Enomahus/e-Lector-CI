import { DatePipe } from '@angular/common';
import {
  Component,
  computed,
  effect,
  inject,
  input,
  OnChanges,
  OnInit,
  output,
  signal,
  SimpleChanges,
} from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { ConstituencyNode } from '@app/models/constituency.model';
import {
  allGenders,
  allMaritalStatus,
  allPersonTitle,
  allRegistrationRequestType,
} from '@app/pages/types/enumerations';
import { CitizenApiService } from '@app/services/api/citizen.api.service';
import { AuthService } from '@app/services/auth/auth.service';
import { PermissionDirective } from '@app/services/auth/permission.directive';
import { BreadcrumbService } from '@app/services/breadcrumb.service';
import { ConstituencyTreeHelperService } from '@app/services/constituency-tree-helper.service';
import {
  Gender,
  GetCitizensResponse,
  GetRegistrationRequestResponse,
  RegistrationRequestModel,
  RegistrationStatus,
} from '@app/services/nswag/api-nswag-client'; //'@app/services/nswag/api-nswag-client';
import { ConstituencyTree } from '@app/shared/constituency-tree/constituency-tree';
import { InputDatepickerUi } from '@app/shared/input-datepicker-ui/input-datepicker-ui';
import { Loader } from '@app/shared/loader/loader';
import { ParentModalUi } from '@app/shared/parent-modal-ui/parent-modal-ui';
import { RegistrationStepResidenceUi } from '@app/shared/registration-step-residence-ui/registration-step-residence-ui';
import { StickyButtonsContainer } from '@app/shared/sticky-buttons-container/sticky-buttons-container';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import {
  createResidenceForm,
  ResidenceForm,
} from '../registration-wizard-ui/registration-wizard-form';
import {
  CitizenForm,
  createRegistrationRequestForm,
  RegistrationRequestForm,
  RequestDocumentsForm,
  RequestsForm,
} from './registration-request-form';

@Component({
  selector: 'app-registration-request-ui',
  imports: [
    TranslateModule,
    FormsModule,
    ReactiveFormsModule,
    ConstituencyTree,
    StickyButtonsContainer,
    Loader,
    PermissionDirective,
    InputDatepickerUi,
    RegistrationStepResidenceUi,
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
  selectedStatus = input<RegistrationStatus | undefined>(undefined);
  saveTriggered = output<{
    registrationRequest: RegistrationRequestModel;
    certificateOfNationalityAttachments?: File;
    cniAttachments?: File;
    photoAttachments?: File;
  }>();
  saveForManagementTriggered = output<RegistrationRequestForm>();
  goBack = output<void>();

  private readonly breadcrumbService = inject(BreadcrumbService);
  private readonly translateService = inject(TranslateService);
  private readonly authService = inject(AuthService);
  private readonly store = inject(ConstituencyTreeHelperService);
  private readonly citizenService = inject(CitizenApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly dialog = inject(MatDialog);
  private readonly datePipe = inject(DatePipe);

  isLoading = signal<boolean>(false);
  isEditMode = signal<boolean>(false);
  form = signal<RegistrationRequestForm>(createRegistrationRequestForm());
  residenceForm = signal<ResidenceForm>(createResidenceForm());
  registrationRequestId = signal<number | undefined>(undefined);
  municipalityId = signal<number | null>(null);

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

  // Computed properties pour filtrer les datalists
  protected filteredFatherOptions = computed(() => this.filterParents(this.fatherInputValue()));
  protected filteredMotherOptions = computed(() => this.filterParents(this.motherInputValue()));

  allRegistrationRequestType = allRegistrationRequestType;
  allMaritalStatus = allMaritalStatus;
  allGenders = allGenders;
  allPersonTitle = allPersonTitle;

  certificateFile = signal<File | null>(null);
  cniFile = signal<File | null>(null);
  photoFile = signal<File | null>(null);
  isSubmitting = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

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
      const id = Number(idParam);
      if (!isNaN(id)) {
        this.registrationRequestId.set(id);
        this.isEditMode.set(true);
      }
    }
    this.loadCitizens();
    this.setBreadcrumbs();
  }

  requestForm(): RequestsForm {
    return this.form().controls.request;
  }
  citizenForm(): CitizenForm {
    return this.form().controls.citizen;
  }
  requestDocumentsForm(): RequestDocumentsForm {
    return this.form().controls.requestDocuments;
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

  private loadCitizens(): void {
    this.citizenService.getCitizens().subscribe((response) => {
      this.parents.set(response.data ?? []);
      this.syncInputValues();
    });
  }

  onNodeSelected(node: ConstituencyNode): void {
    this.store.setSelectedNode(node);
    this.constituencySelected.set([node]);
    this.form().patchValue({ request: { constituencyId: node.id } });
    this.form().controls.request.get('constituencyId')?.markAsDirty();
  }

  private async loadRegistrationRequest(): Promise<void> {
    if (!this.isCreateMode() && this.registrationRequest()) {
      this.form().patchValue(
        {
          request: {
            constituencyId: this.registrationRequest()?.constituencyId,
            registrationType: this.registrationRequest()?.registrationRequestType,
            reasonForRejection: this.registrationRequest()?.reasonForRejection,
          },
          citizen: {
            gender: this.registrationRequest()?.citizen?.gender,
            firstName: this.registrationRequest()?.citizen?.firstName,
            lastName: this.registrationRequest()?.citizen?.lastName,
            birthDate: this.registrationRequest()?.citizen?.birthDate
              ? new Date(this.registrationRequest()?.citizen?.birthDate!)
              : undefined,
            birthPlace: this.registrationRequest()?.citizen?.birthPlace,
            maritalStatus: this.registrationRequest()?.citizen?.maritalStatus,
            marriedName: this.registrationRequest()?.citizen?.marriedName,
            nationality: this.registrationRequest()?.citizen?.nationality,
            profession: this.registrationRequest()?.citizen?.profession,
            physicalAddress: this.registrationRequest()?.citizen?.physicalAddress,
            postalAddress: this.registrationRequest()?.citizen?.postalAddress,
            fatherId: this.registrationRequest()?.citizen?.fatherId,
            motherId: this.registrationRequest()?.citizen?.motherId,
          },
          // registrationCertificateAttachments: this.registrationRequest()?.certificateOfNationalityDocumentIds,
          // registrationCniAttachments: this.registrationRequest()?.identityDocumentIds,
          // photoAttachments: this.registrationRequest()?.photoIds,
        },
        { emitEvent: false },
      );
    }
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

  getRegistrationRequestModel(): RegistrationRequestModel {
    const registrationRequest: RegistrationRequestModel = {
      id: this.form().value.id,
      constituencyId: this.form().value.request?.constituencyId,
      registrationRequestType: this.form().value.request?.registrationType,
      reasonForRejection: this.form().value.request?.reasonForRejection,
      citizen: {
        gender: this.form().value.citizen?.gender,
        firstName: this.form().value.citizen?.firstName,
        lastName: this.form().value.citizen?.lastName,
        birthDate: this.form().value.citizen?.birthDate
          ? new Date(this.form().value.citizen?.birthDate!).toISOString()
          : undefined,
        birthPlace: this.form().value.citizen?.birthPlace,
        maritalStatus: this.form().value.citizen?.maritalStatus,
        marriedName: this.form().value.citizen?.marriedName,
        nationality: this.form().value.citizen?.nationality,
        profession: this.form().value.citizen?.profession,
        physicalAddress: this.form().value.citizen?.physicalAddress,
        postalAddress: this.form().value.citizen?.postalAddress,
        fatherId: this.form().value.citizen?.fatherId,
        motherId: this.form().value.citizen?.motherId,
      },
    };
    return registrationRequest;
  }

  cancel(): void {
    this.goBack.emit();
  }

  onMunicipalitySelected(municipalityId: number | null): void {
    if (!municipalityId) {
      this.constituencySelected.set([]);
      return;
    }

    const constituency = this.store.findNode(municipalityId!);
    this.constituencySelected.set([constituency!]);

    this.form().controls.request.controls.constituencyId.setValue(municipalityId!);
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
        label = this.translateService.instant('breadcrumb.registrationRequestAdd');
      }

      this.breadcrumbService.setBreadcrumbs([
        {
          label: label,
          url: targetRoute,
        },
        {
          label: !this.isEditMode()
            ? this.translateService.instant('breadcrumb.registrationRequestAdd')
            : (this.registrationRequest()?.reference ?? ''),
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

  onFileSelectedTest(event: Event, type: 'certificate' | 'cni' | 'photo'): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] || null;

    if (file && file.size === 0) {
      this.errorMessage.set(`Le fichier pour ${type} ne peut pas être vide.`);
      return;
    }

    this.errorMessage.set(null);

    switch (type) {
      case 'certificate':
        this.certificateFile.set(file);
        this.requestDocumentsForm().controls.registrationCertificateAttachments.setValue(
          file ?? undefined,
        );
        this.requestDocumentsForm().controls.registrationCertificateAttachments.markAsDirty();
        break;
      case 'cni':
        this.cniFile.set(file);
        this.requestDocumentsForm().controls.registrationCniAttachments.setValue(file ?? undefined);
        this.requestDocumentsForm().controls.registrationCniAttachments.markAsDirty();
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
