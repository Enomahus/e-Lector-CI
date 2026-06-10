import { Component, effect, input } from '@angular/core';
import { ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FiliationForm } from '@app/pages/registration-request-home/registration-wizard-ui/registration-wizard-form';
import { MaritalStatus } from '@app/services/nswag/api-nswag-client';

@Component({
  selector: 'app-registration-step-filiation-ui',
  imports: [ReactiveFormsModule, MatFormFieldModule, MatInputModule],
  templateUrl: './registration-step-filiation-ui.html',
  styleUrl: './registration-step-filiation-ui.scss',
})
export class RegistrationStepFiliationUi {
  stepForm = input.required<FiliationForm>();
  situation = input.required<MaritalStatus>();

  constructor() {
    // Ajouter/Retirer les validateurs dynamiquement
    // en fonction du changement de la situation matrimoniale.
    effect(() => {
      const isMarried = this.situation();
      const marriedName = this.stepForm().controls.father;

      if (isMarried) {
        marriedName.setValidators([Validators.required]);
      } else {
        marriedName.clearValidators();
        //marriedName.setValue(null);
      }
      marriedName.updateValueAndValidity();
    });
  }
}
