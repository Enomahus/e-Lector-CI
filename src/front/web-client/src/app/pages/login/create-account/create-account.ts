import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UsersApiService } from '@app/services/api/users.api.service';
import {
  RegisterUserCommand,
  ResultOfError,
  UserModel,
} from '@app/services/nswag/api-nswag-client';
import { UserForm } from '@app/shared/user-form/user-form';
import { createUserForm, UserFormFactory } from '@app/shared/user-form/user-form-factory';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-create-account',
  imports: [TranslateModule, UserForm],
  templateUrl: './create-account.html',
  styleUrls: ['./create-account.scss'],
})
export class CreateAccount implements OnInit {
  private readonly userService = inject(UsersApiService);
  private readonly translateService = inject(TranslateService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  form!: UserFormFactory;
  isSaving = signal(false);
  constituencyId = signal<number | undefined>(undefined);
  email = signal<string | undefined>(undefined);
  // Signal pour gérer la visibilité du mot de passe
  hidePassword = signal(true);
  hideConfirmPassword = signal(true);

  ngOnInit(): void {
    this.email.set(this.route.snapshot.queryParamMap.get('email') ?? undefined);
    const constituencyId = this.route.snapshot.queryParamMap.get('constituencyId');
    this.constituencyId.set(constituencyId ? Number(constituencyId) : undefined);
    this.form = createUserForm(false);
  }

  async onSubmit(user: UserModel): Promise<void> {
    this.isSaving.set(true);
    this.form.controls.email.setErrors(null);

    const command: RegisterUserCommand = { ...user, constituencyId: this.constituencyId() };

    this.userService
      .registerUser(command, {
        successMessage: this.translateService.instant('users.form.registerSuccess'),
        errorMessage: this.translateService.instant('users.form.registerError'),
      })
      .subscribe({
        next: () => {
          this.isSaving.set(false);
        },
        error: (err: ResultOfError) => {
          if (err.data?.code === 'validation' && err.data?.additionalData) {
            for (const field in err.data.additionalData) {
              const validationCode = err.data.additionalData[field];
              if (validationCode === 'unique') {
                this.form.controls.email.setErrors({ unique: true });
              }
            }
          }
          this.isSaving.set(false);
        },
      });
  }

  goBack(): void {
    this.router.navigate(['/login']);
  }
}
