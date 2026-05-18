import { Component, inject, OnInit, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SecurityService } from '@app/services/api/security.service';
import { ResetPasswordCommand } from '@app/services/nswag/api-nswag-client';
import { ToastService } from '@app/services/toast.service';
import { passwordMatchValidator } from '@app/shared/helpers/form.helper';
import { Loader } from '@app/shared/loader/loader';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { LoginPageTemplate } from '../login/login-page-template/login-page-template';

@Component({
  selector: 'app-reset-password-ui',
  imports: [ReactiveFormsModule, LoginPageTemplate, TranslateModule, Loader],
  templateUrl: './reset-password-ui.html',
  styleUrls: ['./reset-password-ui.scss'],
})
export class ResetPasswordUi implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly securityService = inject(SecurityService);
  private readonly translateService = inject(TranslateService);
  private readonly toastService = inject(ToastService);

  // Gestion des états grâce aux Signals (Angular 16+)
  token = signal<string>('');
  email = signal<string>('');
  isLoading = signal<boolean>(false);
  passwordVisible = signal<boolean>(false);
  confirmPasswordVisible = signal<boolean>(false);

  // Formulaire Étape 2
  resetForm = new FormGroup(
    {
      password: new FormControl<string>('', [Validators.required, Validators.minLength(8)]),
      passwordConfirm: new FormControl<string>('', [Validators.required, Validators.minLength(8)]),
    },
    [passwordMatchValidator('password', 'passwordConfirm')],
  );

  ngOnInit(): void {
    const token = this.route.snapshot.queryParamMap.get('token');
    const email = this.route.snapshot.queryParamMap.get('email');
    if (!token || !email) {
      this.toastService.toastError(this.translateService.instant('resetPassword.missingParams'));
      this.router.navigate(['login']);
      return;
    }
    this.token.set(token);
    this.email.set(email);
  }

  // Envoi du nouveau mot de passe
  onResetSubmit(): void {
    if (this.resetForm.invalid) return;

    this.isLoading.set(true);

    const newPassword = this.resetForm.getRawValue().password;
    const currentToken = this.token();

    const resetCommand: ResetPasswordCommand = {
      resetToken: currentToken!,
      password: newPassword!,
      userEmail: this.email()!,
    };

    this.securityService
      .resetPassword(resetCommand, {
        successMessage: this.translateService.instant('resetPassword.resetSuccess'),
        errorMessage: this.translateService.instant('resetPassword.invalidLink'),
      })
      .subscribe({
        next: () => {
          this.isLoading.set(false);
          this.router.navigate(['login']);
        },
        error: () => {
          this.isLoading.set(false);
          this.router.navigate(['login']);
        },
      });
  }

  togglePasswordVisibility(field: 'passwordVisible' | 'confirmPasswordVisible'): void {
    this[field].set(!this[field]());
  }
}
