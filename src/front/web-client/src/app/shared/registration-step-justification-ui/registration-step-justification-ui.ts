import { Component, input } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { RequestDocumentsForm } from '@app/pages/registration-request-home/registration-wizard-ui/registration-wizard-form';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-registration-step-justification-ui',
  imports: [ReactiveFormsModule, MatIconModule, TranslateModule],
  templateUrl: './registration-step-justification-ui.html',
  styleUrl: './registration-step-justification-ui.scss',
})
export class RegistrationStepJustificationUi {
  stepForm = input.required<RequestDocumentsForm>();

  // Gestionnaires d'événements pour le Drag & Drop
  onFileChange(event: Event, controlName: string): void {
    const element = event.currentTarget as HTMLInputElement;
    const filesList: FileList | null = element.files;

    if (filesList && filesList.length > 0) {
      this.stepForm().get(controlName)?.setValue(filesList[0]);
      this.stepForm().get(controlName)?.markAsTouched();
    }
  }
}
