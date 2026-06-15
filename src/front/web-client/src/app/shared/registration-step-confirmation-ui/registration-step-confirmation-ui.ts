import { Component, computed, input, output } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { RegistrationForm } from '@app/pages/registration-request-home/registration-wizard-ui/registration-wizard-form';
import { GetCitizensResponse } from '@app/services/nswag/api-nswag-client';
import { TranslateModule } from '@ngx-translate/core';
import { RecapItem, RecapSectionUi } from '../recap-section-ui/recap-section-ui';

@Component({
  selector: 'app-registration-step-confirmation-ui',
  imports: [ReactiveFormsModule, TranslateModule, RecapSectionUi],
  templateUrl: './registration-step-confirmation-ui.html',
  styleUrl: './registration-step-confirmation-ui.scss',
})
export class RegistrationStepConfirmationUi {
  data = input.required<RegistrationForm>();
  isSubmitting = input<boolean>(false);
  citizens = input<GetCitizensResponse[] | null>(null);
  editStep = output<number>();
  submitRequest = output<void>();

  getRequestData() {
    return {
      type: this.data().value.request?.registrationType,
      comment: this.data().value.request?.reasonForRejection,
    };
  }

  private readonly GENRE_LABELS: Record<string, string> = { m: 'Masculin', f: 'Féminin' };
  private readonly SITUATION_LABELS: Record<string, string> = {
    single: 'Célibataire',
    married: 'Marié(e)',
    divorced: 'Divorcé(e)',
    widowed: 'Veuf/Veuve',
  };

  protected identityItems = computed<RecapItem[]>(() => {
    const raw = this.data().value.identity || {};
    return [
      { label: 'Genre', value: this.GENRE_LABELS[raw.gender!] },
      { label: 'Nom de famille', value: raw.lastName! },
      { label: 'Prénom', value: raw.firstName! },
      { label: 'Date de naissance', value: raw.birthDate?.toString() },
      { label: 'Lieu de naissance', value: raw.birthPlace! },
      { label: 'Situation matrimoniale', value: this.SITUATION_LABELS[raw.maritalStatus!] },
      { label: 'Nom de jeune fille', value: raw.marriedName },
      { label: 'Nationalité', value: raw.nationality },
    ];
  });

  protected filiationItems = computed<RecapItem[]>(() => {
    const raw = this.data().value.filiation || {};
    const father = this.citizens()?.find((f) => f.id === raw.fatherId);
    const mother = this.citizens()?.find((f) => f.id === raw.motherId);
    return [
      { label: 'Nom du père', value: `${father?.firstName} ${father?.lastName}` },
      //{ label: 'Prénom du père', value: raw.prenomPere },
      //{ label: 'Nationalité du père', value: raw.nationalitePere },
      { label: 'Nom de la mère', value: `${mother?.firstName} ${mother?.lastName}` },
      // { label: 'Prénom de la mère', value: raw.prenomPere },
      // { label: 'Nationalité de la mère', value: raw.nationaliteMere },
    ];
  });

  protected coordonneesItems = computed<RecapItem[]>(() => {
    const raw = this.data().value.coordinates || {};
    return [
      { label: 'E-mail', value: raw.email },
      { label: 'Profession', value: raw.profession },
    ];
  });

  protected residenceItems = computed<RecapItem[]>(() => {
    const raw = this.data().value.residence || {};
    //const adresseComplete = `${raw.adresse || ''}${raw.complementAdresse ? ', ' + raw.complementAdresse : ''}`;
    return [
      { label: 'Adresse postal', value: raw.postalAddress || null },
      { label: 'Adresse physique', value: raw.physicalAddress },
      { label: 'Commune', value: raw.municipalityId },
      { label: 'Sous-Préfecture', value: raw.subPrefectureId },
      { label: 'Département', value: raw.departmentId },
      { label: 'Région', value: raw.regionId },
    ];
  });

  protected justificatifsItems = computed<RecapItem[]>(() => {
    const raw = this.data().value.requestDocuments || {};
    return [
      { label: 'Type de pièce', value: raw.typeOfIdentificationDocument },
      { label: 'Numéro de pièce', value: raw.identificationDocumentNumber },
      { label: 'Date de délivrance', value: raw.issueDate?.toString() },
      { label: "Date d'expiration", value: raw.expiryDate?.toString() },
      { label: 'Lieu de délivrance', value: raw.issuePlace },
      { label: "Pièce d'identité", value: raw.registrationCniAttachments?.name },
      { label: 'Certficat de nationalité', value: raw.registrationCertificateAttachments?.name },
      { label: 'Photo', value: raw.photoAttachments?.name },
      { label: 'Consentement données', value: raw.isRgpdConsent },
    ];
  });
}
