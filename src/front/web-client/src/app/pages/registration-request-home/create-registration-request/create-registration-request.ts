import { CommonModule, JsonPipe } from '@angular/common';
import { Component, signal } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import {
  allGenders,
  allMaritalStatus,
  allPersonTitle,
  allRegistrationRequestType,
} from '@app/pages/types/enumerations';
import { Gender } from '@app/services/nswag/api-nswag-client';
import { InputDatepickerUi } from '@app/shared/input-datepicker-ui/input-datepicker-ui';
import { TranslateModule } from '@ngx-translate/core';
import { createRegistrationForm, FiliationForm, IdentityForm } from './registration-form';

@Component({
  selector: 'app-create-registration-request',
  imports: [CommonModule, ReactiveFormsModule, InputDatepickerUi, TranslateModule, JsonPipe],
  templateUrl: './create-registration-request.html',
  styleUrl: './create-registration-request.scss',
})
export class CreateRegistrationRequest {
  allRegistrationRequestType = allRegistrationRequestType;
  allMaritalStatus = allMaritalStatus;
  allGenders = allGenders;
  allPersonTitle = allPersonTitle;

  steps = signal([
    { id: 1, name: 'Identité', state: 'active' },
    { id: 2, name: 'Filiation', state: 'inactive' },
    { id: 3, name: 'Coordonnées', state: 'inactive' },
    { id: 4, name: 'Résidence', state: 'inactive' },
    { id: 5, name: 'Justificatifs', state: 'inactive' },
    { id: 6, name: 'Confirmation', state: 'inactive' },
  ]);

  nationalities = signal(['Ivoirienne', 'Naturalisé']);
  form = createRegistrationForm();

  setGender(gender: Gender) {
    this.form.controls.identity.controls.gender.setValue(gender);
  }
  identityForm(): IdentityForm {
    return this.form.controls.identity;
  }
  filiatioForm(): FiliationForm {
    return this.form.controls.filisation;
  }

  onPrevious() {
    console.log("Retourner à l'étape précédente");
  }
  onNext() {
    if (this.form.controls.identity.valid) {
      console.log("Soumettre le formulaire de l'étape 1 et passer à l'étape 2", this.form.value);
      // Logique pour passer à l'étape suivante
    } else {
      console.log('Le formulaire est invalide');
      this.form.controls.identity.markAllAsTouched();
    }
  }
  validateStep(step: number): void {
    if (step === 1) {
    }
  }
}
