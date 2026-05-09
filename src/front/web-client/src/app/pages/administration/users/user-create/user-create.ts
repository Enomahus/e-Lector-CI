import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Breadcrumbs } from '@app/models/breadcrumb.model';
import { UsersApiService } from '@app/services/api/users.api.service';
import { BreadcrumbService } from '@app/services/breadcrumb.service';
import { CreateUserCommand, UserModel } from '@app/services/nswag/api-nswag-client';
import { UserForm } from '@app/shared/user-form/user-form';
import { createUserForm, UserFormFactory } from '@app/shared/user-form/user-form-factory';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-user-create',
  imports: [UserForm, TranslateModule],
  templateUrl: './user-create.html',
  styleUrls: ['./user-create.scss'],
})
export class UserCreate implements OnInit {
  form = signal<UserFormFactory>(createUserForm());
  isSaving = signal(false);
  constituencyId = signal<number | undefined>(undefined);

  private readonly breadcrumbService = inject(BreadcrumbService);
  private readonly translateService = inject(TranslateService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly userService = inject(UsersApiService);

  ngOnInit(): void {
    const constituencyId = this.route.snapshot.queryParamMap.get('constituencyId');
    this.constituencyId.set(constituencyId ? Number(constituencyId) : undefined);
    this.setBreadcrumb();
  }

  private setBreadcrumb(): void {
    let breadcrumbs: Breadcrumbs[] = [];
    breadcrumbs = [
      {
        label: this.translateService.instant('breadcrumb.users'),
        url: `/admin/users`,
      },
      {
        label: this.translateService.instant('breadcrumb.userCreate'),
      },
    ];
    this.breadcrumbService.setBreadcrumbs(breadcrumbs);
  }

  onSubmit(user: UserModel): void {
    this.isSaving.set(true);

    const command: CreateUserCommand = { ...user };
    this.userService
      .createUser(command, {
        successMessage: this.translateService.instant('users.successCreating'),
        errorMessage: this.translateService.instant('users.errorCreating'),
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
