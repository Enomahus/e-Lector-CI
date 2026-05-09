import { Component, inject, OnInit, signal } from '@angular/core';
import { UsersApiService } from '@app/services/api/users.api.service';
import { AuthService } from '@app/services/auth/auth.service';
import { CurrentUserService } from '@app/services/current-user.service';
import { GetCurrentUserResponse, UserModel } from '@app/services/nswag/api-nswag-client';
import { Loader } from '@app/shared/loader/loader';
import { UserForm } from '@app/shared/user-form/user-form';
import { createUserForm, updateEmployeeValidators } from '@app/shared/user-form/user-form-factory';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-my-account-ui',
  imports: [TranslateModule, UserForm, Loader],
  templateUrl: './my-account-ui.html',
  styleUrl: './my-account-ui.scss',
})
export class MyAccountUi implements OnInit {
  private readonly userService = inject(UsersApiService);
  private readonly authService = inject(AuthService);
  private readonly currentUserService = inject(CurrentUserService);
  private readonly translateService = inject(TranslateService);

  constituencyId = signal<number | undefined>(undefined);
  isSaving = signal(false);
  isLoading = signal(false);
  form = createUserForm();
  user = signal<GetCurrentUserResponse | undefined>(undefined);

  ngOnInit(): void {
    this.isLoading.set(true);
    this.userService.getCurrentUser().subscribe({
      next: (userInfo) => {
        this.form.patchValue(
          {
            civility: userInfo.civility,
            firstName: userInfo.firstName,
            lastName: userInfo.lastName,
            phone: userInfo.phoneNumber,
            //password: userInfo.password,
            //confirmPassword: userInfo.password,
            constituencyId: userInfo.constituencyId,
            employeeNumber: '', //TODO: handle employee number in my account
            roles: ['agent', 'demandeur'], //TODO: handle roles in my account
            email: userInfo.email,
            authProvider: userInfo.authProvider,
          },
          { emitEvent: false },
        );
        updateEmployeeValidators(this.form);
        this.user.set(userInfo);
        this.constituencyId.set(userInfo.constituencyId);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      },
    });
  }

  updateMyAccountInformations(userInfo: UserModel): void {
    this.isSaving.set(true);
    this.userService
      .udpateCurrentUser(userInfo, {
        successMessage: this.translateService.instant('users.form.updateMyInformationsSuccess'),
        errorMessage: this.translateService.instant('users.form.updateError'),
      })
      .subscribe({
        next: () => {
          this.isSaving.set(false);
          this.currentUserService.changeCurrentUserName(
            `${userInfo.firstName} ${userInfo.lastName}`,
          );
          if (userInfo.email !== this.authService.getCurrentUserEmail()) {
            this.authService.logout();
            window.location.reload();
          }
        },
        error: () => {
          this.isSaving.set(false);
        },
      });
  }

  reloadUi(): void {
    window.location.reload();
  }
}
