import { Component, input } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { CoordinatesForm } from '@app/pages/registration-request-home/registration-wizard-ui/registration-wizard-form';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-registration-step-coordinates-ui',
  imports: [ReactiveFormsModule, TranslateModule],
  templateUrl: './registration-step-coordinates-ui.html',
  styleUrl: './registration-step-coordinates-ui.scss',
})
export class RegistrationStepCoordinatesUi {
  stepForm = input.required<CoordinatesForm>();
}
