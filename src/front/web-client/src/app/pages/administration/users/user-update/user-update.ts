import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { Breadcrumbs } from '@app/models/breadcrumb.model';
import { UsersApiService } from '@app/services/api/users.api.service';
import { BreadcrumbService } from '@app/services/breadcrumb.service';
import { UpdateUserCommand, UserModel } from '@app/services/nswag/api-nswag-client';
import { Loader } from '@app/shared/loader/loader';
import { UserForm } from '@app/shared/user-form/user-form';
import { createUserForm, UserFormFactory } from '@app/shared/user-form/user-form-factory';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { map, switchMap, tap } from 'rxjs';

@Component({
  selector: 'app-user-update',
  imports: [UserForm, TranslateModule, Loader],
  templateUrl: './user-update.html',
  styleUrls: ['./user-update.scss'],
})
export class UserUpdate implements OnInit {
  form = signal<UserFormFactory>(createUserForm(true));
  isSaving = signal(false);
  constituencyId = signal<number | undefined>(undefined);
  user = signal<UserModel | null>(null);
  userId = signal<string | undefined>(undefined);

  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly userService = inject(UsersApiService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly breadcrumbService = inject(BreadcrumbService);
  private readonly translateService = inject(TranslateService);

  ngOnInit(): void {
    this.route.params
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        map((param) => param['id']),
        tap((id) => this.userId.set(id)),
        switchMap((id) => this.userService.getUser(id)),
        tap((user) => {
          this.user.set(user);
          this.constituencyId.set(user.constituencyId);
          this.loadUser(user);
        }),
      )
      .subscribe();

    this.setBreadcrumb();
  }

  private loadUser(user: UserModel): void {
    this.form().patchValue(
      {
        civility: user.civility || 'mr',
        lastName: user.lastName,
        firstName: user.firstName,
        phone: user.phoneNumber,
        email: user.email,
        //password: user.password,
        // confirmPassword: user.password,
        employeeNumber: user.employeeNumber,
        roles: user.roles,
        authProvider: user.authProvider,
        constituencyId: user.constituencyId,
      },
      { emitEvent: false },
    );
  }

  private setBreadcrumb(): void {
    let breadcrumbs: Breadcrumbs[] = [];

    breadcrumbs = [
      {
        label: this.translateService.instant('breadcrumb.users'),
        url: `/admin/users`,
      },
      {
        label: this.translateService.instant('breadcrumb.userEdit'),
      },
    ];
    this.breadcrumbService.setBreadcrumbs(breadcrumbs);
  }

  async onSubmit(user: UserModel): Promise<void> {
    if (!this.userId()) {
      return;
    }
    this.isSaving.set(true);

    const command: UpdateUserCommand = { ...user };

    this.userService
      .udpateUser(this.userId()!, command, {
        successMessage: this.translateService.instant('users.updateSuccess'),
        errorMessage: this.translateService.instant('users.updateError'),
      })
      .subscribe({
        next: () => {
          this.isSaving.set(false);
          this.router.navigate(['admin', 'users']);
        },
        error: () => {
          this.isSaving.set(false);
        },
      });
  }

  goBack(): void {
    this.router.navigate(['admin', 'users']);
  }
}
