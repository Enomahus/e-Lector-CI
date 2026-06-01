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
import { ActivatedRoute, Router } from '@angular/router';
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
  GetCitizensResponse,
  GetRegistrationRequestResponse,
  RegistrationRequestModel,
  RegistrationStatus,
} from '@app/services/nswag/api-nswag-client';
import { ConstituencyTree } from '@app/shared/constituency-tree/constituency-tree';
import { Loader } from '@app/shared/loader/loader';
import { StickyButtonsContainer } from '@app/shared/sticky-buttons-container/sticky-buttons-container';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import {
  CitizenForm,
  createRegistrationRequestForm,
  RegistrationRequestForm,
  RequestDocumentsForm,
  RequestsForm,
} from './registration-request-form';

// On assume que ton parent ressemble à ça
interface ParentModel {
  id: string;
  firstName: string;
  lastName: string;
  birthDate: string | Date;
  birthPlace: string;
}

@Component({
  selector: 'app-registration-request-ui',
  imports: [
    TranslateModule,
    FormsModule,
    ReactiveFormsModule,
    PermissionDirective,
    ConstituencyTree,
    StickyButtonsContainer,
    Loader,
    DatePipe,
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
  private readonly router = inject(Router);

  isLoading = signal<boolean>(false);
  isEditMode = signal<boolean>(false);
  form = signal<RegistrationRequestForm>(createRegistrationRequestForm());
  registrationRequestId = signal<number | undefined>(undefined);

  nodes = this.store.nodesData;
  selectedNode = this.store.selectedNode;
  initialTreeSelectedId = computed(() =>
    !this.isCreateMode() ? this.constituencyId() : undefined,
  );

  constituencySelected = signal<ConstituencyNode[]>([]);
  parents = signal<GetCitizensResponse[]>([]);

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

  private loadCitizens(): void {
    this.citizenService.getCitizens().subscribe((response) => {
      this.parents.set(response.data ?? []);
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

      if (permissions.some((p) => p === 'createRegistrationRequest')) {
        label = this.translateService.instant('breadcrumb.registrationRequestAdd');
      }
      //TODO: Ajout des autres access ForManagement et ForAdmin

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

  protected fatherSearchQuery = signal<string>('');
  protected motherSearchQuery = signal<string>('');

  protected filteredParents = computed(() => {
    const query = this.fatherSearchQuery().toLowerCase().trim();
    const allParents = this.parents(); // Ton signal parent initial

    if (!query) return allParents;

    return allParents.filter(
      (p) =>
        p.firstName?.toLowerCase().includes(query) ||
        p.lastName?.toLowerCase().includes(query) ||
        p.birthPlace?.toLowerCase().includes(query),
    );
  });

  protected selectedFatherName = computed(() => {
    const currentId = this.citizenForm().controls.fatherId.value;
    if (!currentId) return '';

    const found = this.parents().find((p) => p.id === currentId);
    return found ? `${found.firstName} ${found.lastName}` : '';
  });

  protected selectedMotherName = computed(() => {
    const currentId = this.citizenForm().controls.motherId.value;
    if (!currentId) return '';

    const found = this.parents().find((p) => p.id === currentId);
    return found ? `${found.firstName} ${found.lastName}` : '';
  });

  protected onFatherSearchChange(event: Event): void {
    const inputVal = (event.target as HTMLInputElement).value;
    this.fatherSearchQuery.set(inputVal);

    // Recherche si la valeur saisie ou sélectionnée correspond à un parent existant
    const matchedParent = this.parents().find((p) => {
      const fullNameString = `${p.firstName} ${p.lastName}`.toLowerCase();
      return inputVal.toLowerCase().startsWith(fullNameString);
    });

    if (matchedParent) {
      // Si correspondance trouvée (clic dans la liste ou saisie complète), on pousse l'ID dans le formulaire
      this.citizenForm().controls.fatherId.setValue(matchedParent.id);
      this.citizenForm().controls.fatherId.markAsDirty();
    } else {
      // Si l'utilisateur efface ou tape un texte incomplet, l'ID redevient nul
      this.citizenForm().controls.fatherId.setValue('');
    }
  }

  protected onParentSearchChange(event: Event, parentType: 'father' | 'mother'): void {
    const inputVal = (event.target as HTMLInputElement).value;
    const lowerInputVal = inputVal.toLowerCase();

    if (parentType === 'father') {
      this.fatherSearchQuery.set(inputVal);
    } else {
      this.motherSearchQuery.set(inputVal);
    }

    const matchedParent = this.parents().find((p) => {
      const fullNameString = `${p.firstName} ${p.lastName}`.toLowerCase();
      return lowerInputVal.startsWith(fullNameString);
    });

    const controlName = `${parentType}Id`;
    const parentControl = this.citizenForm().get(controlName);

    if (parentControl) {
      if (matchedParent) {
        parentControl.setValue(matchedParent.id);
        parentControl.markAsDirty();
      } else {
        parentControl.setValue('');
      }
    }
  }

  /**
   * Gestionnaire d'événements pour la sélection de fichiers
   */
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
}
