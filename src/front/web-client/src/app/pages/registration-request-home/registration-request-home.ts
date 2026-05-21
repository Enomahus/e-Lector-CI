import { Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { AuthService } from '@app/services/auth/auth.service';
import { HideIfAdminDirective } from '@app/services/auth/hide-if-admin.directive';
import { PermissionDirective } from '@app/services/auth/permission.directive';
import { AppPermission } from '@app/services/nswag/api-nswag-client';
import { TranslateModule } from '@ngx-translate/core';
import { RegistrationRequestsForAdminUi } from './registration-requests-for-admin-ui/registration-requests-for-admin-ui';
import { RegistrationRequestsForManagementUi } from './registration-requests-for-management-ui/registration-requests-for-management-ui';
import { RegistrationRequestsUi } from './registration-requests-ui/registration-requests-ui';

@Component({
  selector: 'app-registration-request-home',
  imports: [
    TranslateModule,
    HideIfAdminDirective,
    PermissionDirective,
    RegistrationRequestsForAdminUi,
    RegistrationRequestsForManagementUi,
    RegistrationRequestsUi,
  ],
  templateUrl: './registration-request-home.html',
  styleUrls: ['./registration-request-home.scss'],
})
export class RegistrationRequestHome {
  private readonly authService = inject(AuthService);

  private readonly permissions = toSignal(this.authService.getPermissions(), {
    initialValue: [] as AppPermission[],
  });

  readonly showAdminRequestsText = computed(() =>
    this.permissions().includes('accessRegistrationRequestsForAdminPage'),
  );
  readonly showOrganismRequestsText = computed(() =>
    this.permissions().includes('accessRegistrationRequestsForManagementPage'),
  );
  readonly showElectorRequestsText = computed(() =>
    this.permissions().includes('accessRegistrationRequestsPage'),
  );
}
