import { Component, input } from '@angular/core';
import { CoordinatesForm } from '@app/pages/registration-request-home/registration-wizard-ui/registration-wizard-form';

@Component({
  selector: 'app-registration-step-coordinates-ui',
  imports: [],
  templateUrl: './registration-step-coordinates-ui.html',
  styleUrl: './registration-step-coordinates-ui.scss',
})
export class RegistrationStepCoordinatesUi {
  stepForm = input.required<CoordinatesForm>();
}
