import { Component, input } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { CoordinatesForm } from '../../pages/registration-request-home/registration-wizard-ui/registration-wizard-form';

@Component({
  selector: 'app-registration-step-coordinates-ui',
  imports: [ReactiveFormsModule, TranslateModule],
  templateUrl: './registration-step-coordinates-ui.html',
  styleUrl: './registration-step-coordinates-ui.scss',
})
export class RegistrationStepCoordinatesUi {
  stepForm = input.required<CoordinatesForm>();
}
