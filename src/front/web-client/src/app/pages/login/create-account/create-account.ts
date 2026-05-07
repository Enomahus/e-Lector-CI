import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { PhoneInput } from '@app/shared/phone-input/phone-input';
import { StickyButtonsContainer } from '@app/shared/sticky-buttons-container/sticky-buttons-container';
import { TranslateModule } from '@ngx-translate/core';
import { AccountForm, createAccountForm } from './account-form';

export type Activity = 'demandeur' | 'agent' | 'administrateur';

@Component({
  selector: 'app-create-account',
  imports: [TranslateModule, CommonModule, ReactiveFormsModule, StickyButtonsContainer, PhoneInput],
  templateUrl: './create-account.html',
  styleUrl: './create-account.scss',
})
export class CreateAccount {
  private readonly router = inject(Router);

  activityOptions: Activity[] = ['demandeur', 'agent', 'administrateur'];

  registerForm: AccountForm = createAccountForm();
  // Signal pour gérer la visibilité du mot de passe
  hidePassword = signal(true);
  hideConfirmPassword = signal(true);

  roleSelected = toSignal(this.registerForm.controls.roles.valueChanges, {
    initialValue: this.registerForm.controls.roles.value,
  });

  isRequester = computed(() => this.roleSelected().includes('demandeur'));

  isFieldInvalid(fieldName: string): boolean {
    const control = this.registerForm.get(fieldName);
    return !!(control && control.invalid && (control.touched || control.dirty));
  }

  togglePassword() {
    this.hidePassword.update((v) => !v);
  }

  toggleConfirmPassword() {
    this.hideConfirmPassword.update((v) => !v);
  }

  onSubmit() {
    if (this.registerForm.valid) {
      console.log('Formulaire soumis:', this.registerForm.value);
    }
  }
  goBack(): void {
    this.router.navigate(['/login']);
  }
}
