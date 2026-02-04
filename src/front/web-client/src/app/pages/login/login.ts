import { Component, inject, signal } from '@angular/core';
import { LoginPageTemplate } from './login-page-template/login-page-template';
import { Router, RouterLink } from '@angular/router';
import { email, form, FormField, maxLength, minLength, required, submit } from '@angular/forms/signals';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Loader } from '../../shared/loader/loader';

@Component({
  selector: 'app-login',
  imports: [LoginPageTemplate, Loader, RouterLink, TranslateModule, FormField],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  private readonly router = inject(Router);
  private readonly translateService = inject(TranslateService);

  showPassword = signal(false);
  isLoggingIn = signal(false);

  loginModel = signal({
    email: '',
    password: ''
  });

  loginForm = form(this.loginModel, (schemaPath) => {
    required(schemaPath.email, { message: this.translateService.instant('formError.required') });
    email(schemaPath.email, { message: this.translateService.instant('formError.emailInvalid') });
    required(schemaPath.password, { message: this.translateService.instant('formError.required') });
    minLength(schemaPath.password, 8, { message: this.translateService.instant('formError.passwordFormat') });
    maxLength(schemaPath.password, 24, { message: this.translateService.instant('formError.passwordFormat') });
  });

  onSubmit(): void {
    //event.stopPropagation();
    this.isLoggingIn.set(true);

    const creadentials = this.loginModel();

    if(this.loginForm().invalid()) {
      this.loginForm().markAsTouched();
      return;
    }

    console.log('Logging with :', creadentials);
    //this.router.navigate(['/home']);

  }

  
}
