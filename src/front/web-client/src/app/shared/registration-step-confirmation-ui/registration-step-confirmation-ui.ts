import { Component, input, output } from '@angular/core';
import { RegistrationForm } from '@app/pages/registration-request-home/registration-wizard-ui/registration-wizard-form';

@Component({
  selector: 'app-registration-step-confirmation-ui',
  imports: [],
  templateUrl: './registration-step-confirmation-ui.html',
  styleUrl: './registration-step-confirmation-ui.scss',
})
export class RegistrationStepConfirmationUi {
  data = input.required<RegistrationForm>();
  editStep = output<number>();
}
