import { computed, inject, Injectable, signal } from '@angular/core';
import { NonNullableFormBuilder, Validators } from '@angular/forms';

@Injectable({
  providedIn: 'root',
})
export class InscriptionService {
  private fb = inject(NonNullableFormBuilder);

  // Signaux pour l'état de la navigation
  readonly currentStep = signal<number>(1);
  readonly totalSteps = 6;

  // Signaux calculés (Read-only pour les composants)
  readonly isFirstStep = computed(() => this.currentStep() === 1);
  readonly isLastStep = computed(() => this.currentStep() === this.totalSteps);

  // Formulaire principal typé et centralisé
  readonly mainForm = this.fb.group({
    identite: this.fb.group({
      nom: ['', [Validators.required, Validators.minLength(2)]],
      prenoms: ['', Validators.required],
      dateNaissance: ['', Validators.required],
    }),
    filiation: this.fb.group({
      nomPere: ['', Validators.required],
      nomMere: ['', Validators.required],
    }),
    coordonnees: this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      telephone: ['', [Validators.required, Validators.pattern(/^\+?[0-9]{8,15}$/)]],
    }),
    residence: this.fb.group({
      adresse: ['', Validators.required],
      ville: ['', Validators.required],
      pays: ["Côte d'Ivoire", Validators.required],
    }),
    justificatifs: this.fb.group({
      pieceIdentite: [null as File | null, Validators.required],
      justificatifDomicile: [null as File | null, Validators.required],
    }),
  });

  nextStep(): void {
    if (this.currentStep() < this.totalSteps && this.isCurrentStepValid()) {
      this.currentStep.update((step) => step + 1);
    }
  }

  previousStep(): void {
    if (!this.isFirstStep()) {
      this.currentStep.update((step) => step - 1);
    }
  }

  // Validation dynamique par bloc d'étape
  private isCurrentStepValid(): boolean {
    const stepsControls = ['identite', 'filiation', 'coordonnees', 'residence', 'justificatifs'];
    const currentControlName = stepsControls[this.currentStep() - 1];

    if (currentControlName) {
      const control = this.mainForm.get(currentControlName);
      control?.markAllAsTouched();
      return control?.valid ?? false;
    }
    return true; // Étape de confirmation
  }
}
