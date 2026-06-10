import { Component, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatStepperModule } from '@angular/material/stepper';
import { allRegistrationRequestType } from '@app/pages/types/enumerations';
import { RegistrationStepConfirmationUi } from '@app/shared/registration-step-confirmation-ui/registration-step-confirmation-ui';
import { RegistrationStepCoordinatesUi } from '@app/shared/registration-step-coordinates-ui/registration-step-coordinates-ui';
import { RegistrationStepFiliationUi } from '@app/shared/registration-step-filiation-ui/registration-step-filiation-ui';
import { RegistrationStepIdentityUi } from '@app/shared/registration-step-identity-ui/registration-step-identity-ui';
import { RegistrationStepJustificationUi } from '@app/shared/registration-step-justification-ui/registration-step-justification-ui';
import { RegistrationStepResidenceUi } from '@app/shared/registration-step-residence-ui/registration-step-residence-ui';
import { TranslateModule } from '@ngx-translate/core';
import { createRegistrationForm, RegistrationForm, RequestForm } from './registration-wizard-form';

@Component({
  selector: 'app-registration-wizard-ui',
  imports: [
    ReactiveFormsModule,
    TranslateModule,
    MatStepperModule,
    MatButtonModule,
    RegistrationStepIdentityUi,
    RegistrationStepFiliationUi,
    RegistrationStepCoordinatesUi,
    RegistrationStepResidenceUi,
    RegistrationStepJustificationUi,
    RegistrationStepConfirmationUi,
  ],
  templateUrl: './registration-wizard-ui.html',
  styleUrl: './registration-wizard-ui.scss',
})
export class RegistrationWizardUi {
  isSubmitted = signal(false);
  isSubmitting = signal(false);
  formData = signal<RegistrationForm | null>(null);

  form = createRegistrationForm();
  allRegistrationRequestType = allRegistrationRequestType;

  requestForm(): RequestForm {
    return this.form.controls.request;
  }

  // Signal dérivé depuis RxJS pour passer la situation matrimoniale à l'étape 2
  situationMatrimoniale = toSignal(
    this.form.controls.identity.controls.maritalStatus.valueChanges,
    { initialValue: this.form.controls.identity.controls.maritalStatus.value },
  );

  submitForm() {
    if (this.form.invalid) return;

    this.isSubmitting.set(true);
    // Simulation API
    setTimeout(() => {
      //this.formData.set(this.form.getRawValue());
      this.isSubmitted.set(true);
      this.isSubmitting.set(false);
    }, 1500);
  }

  resetForm() {
    this.form.reset();
    this.isSubmitted.set(false);
  }
}
