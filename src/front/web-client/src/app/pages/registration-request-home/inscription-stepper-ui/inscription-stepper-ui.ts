import { JsonPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { InscriptionService } from '@app/services/inscription.service';

@Component({
  selector: 'app-inscription-stepper-ui',
  imports: [ReactiveFormsModule, JsonPipe],
  templateUrl: './inscription-stepper-ui.html',
  styleUrl: './inscription-stepper-ui.scss',
})
export class InscriptionStepperUi {
  protected inscriptionService = inject(InscriptionService);

  protected form = this.inscriptionService.mainForm;

  onSubmit(): void {
    if (this.form.valid) {
      const payload = this.form.value;
      console.log("Formulaire d'inscription soumis avec succès :", payload);
      // Appel vers une API de traitement ici
    } else {
      this.form.markAllAsTouched();
    }
  }

  // Assurez-vous d'avoir importé le type InscriptionService dans le composant
  // si vous souhaitez typer finement le ciblage du contrôle.
  onFileChange(event: Event, controlName: 'pieceIdentite' | 'justificatifDomicile'): void {
    const input = event.target as HTMLInputElement;

    if (input?.files && input.files.length > 0) {
      const file = input.files[0];

      // On récupère directement le contrôle spécifique
      const control = this.form.get(`justificatifs.${controlName}`);

      if (control) {
        control.setValue(file);
        control.markAsDirty();
        control.updateValueAndValidity();
      }
    }
  }
}
