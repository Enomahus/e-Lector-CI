import { Component, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { RequestDocumentsForm } from '@app/pages/registration-request-home/registration-wizard-ui/registration-wizard-form';

@Component({
  selector: 'app-registration-step-justification-ui',
  imports: [MatIconModule],
  templateUrl: './registration-step-justification-ui.html',
  styleUrl: './registration-step-justification-ui.scss',
})
export class RegistrationStepJustificationUi {
  stepForm = input.required<RequestDocumentsForm>();
}
