import { Component, input } from '@angular/core';
import { ResidenceForm } from '@app/pages/registration-request-home/registration-wizard-ui/registration-wizard-form';

@Component({
  selector: 'app-registration-step-residence-ui',
  imports: [],
  templateUrl: './registration-step-residence-ui.html',
  styleUrl: './registration-step-residence-ui.scss',
})
export class RegistrationStepResidenceUi {
  stepForm = input.required<ResidenceForm>();
}
