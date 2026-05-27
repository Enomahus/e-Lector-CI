import { Routes } from '@angular/router';
import { Constituencies } from './pages/administration/constituencies/constituencies';
import { ConstituencyCreate } from './pages/administration/constituencies/constituency-create/constituency-create';
import { ConstituencyUpdate } from './pages/administration/constituencies/constituency-update/constituency-update';
import { PollingStationCreate } from './pages/administration/polling-stations/polling-station-create/polling-station-create';
import { PollingStationUpdate } from './pages/administration/polling-stations/polling-station-update/polling-station-update';
import { PollingStations } from './pages/administration/polling-stations/polling-stations';
import { UserCreate } from './pages/administration/users/user-create/user-create';
import { UserUpdate } from './pages/administration/users/user-update/user-update';
import { Users } from './pages/administration/users/users';
import { FaqUi } from './pages/faq-ui/faq-ui';
import { ForgotPassword } from './pages/forgot-password/forgot-password';
import { Home } from './pages/home/home';
import { CreateAccount } from './pages/login/create-account/create-account';
import { Login } from './pages/login/login';
import { MyAccountUi } from './pages/my-account-ui/my-account-ui';
import { CreateRegistrationRequestUi } from './pages/registration-request-home/create-registration-request-ui/create-registration-request-ui';
import { RegistrationRequestsForAdminUi } from './pages/registration-request-home/registration-requests-for-admin-ui/registration-requests-for-admin-ui';
import { RegistrationRequestsForManagementUi } from './pages/registration-request-home/registration-requests-for-management-ui/registration-requests-for-management-ui';
import { RegistrationRequestsUi } from './pages/registration-request-home/registration-requests-ui/registration-requests-ui';
import { UpdateRegistrationRequestUi } from './pages/registration-request-home/update-registration-request-ui/update-registration-request-ui';
import { ResetPasswordUi } from './pages/reset-password-ui/reset-password-ui';
import { PermissionGuard } from './services/auth/permission.guard';
import { AppPermission } from './services/nswag/api-nswag-client';
import { PageTemplate } from './shared/page-template/page-template';

export function perm(p: AppPermission): AppPermission {
  return p;
}

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/login',
    pathMatch: 'full',
  },
  {
    path: 'login',
    component: Login,
    title: 'login.title',
  },
  {
    path: 'login/:provider',
    component: Login,
    title: 'login.title',
  },
  {
    path: 'faq',
    component: FaqUi,
    title: 'faq.title',
  },
  {
    path: 'reset-password',
    component: ResetPasswordUi,
    title: 'resetPassword.resetPassword',
  },
  {
    path: 'forgot-password',
    component: ForgotPassword,
    title: 'forgotPassword.forgotPassword',
  },
  {
    path: '',
    component: PageTemplate,
    children: [
      {
        path: 'home',
        component: Home,
        canActivate: [PermissionGuard],
        title: 'home.title',
      },
      {
        path: 'register',
        component: CreateAccount,
        title: 'register.title',
        canActivate: [PermissionGuard],
      },
      {
        path: 'registration-requests',
        component: RegistrationRequestsUi,
        canActivate: [PermissionGuard],
        title: 'registrationRequests.title',
      },
      {
        path: 'registration-requests-for-management',
        component: RegistrationRequestsForManagementUi,
        canActivate: [PermissionGuard],
        title: 'registrationRequests.titleForManagement',
      },
      {
        path: 'registration-requests-for-admin',
        component: RegistrationRequestsForAdminUi,
        canActivate: [PermissionGuard],
        title: 'registrationRequests.titleForAdmin',
      },
      {
        path: 'registration-requests/new',
        component: CreateRegistrationRequestUi,
        canActivate: [PermissionGuard],
        data: {
          permission: perm('createRegistrationRequest'),
        },
        title: 'registrationRequests.titleNewRegistrationRequest',
      },
      {
        path: 'registration-requests/:id',
        component: UpdateRegistrationRequestUi,
        canActivate: [PermissionGuard],
        data: {
          permission: perm('updateRegistrationRequest'),
        },
        title: 'registrationRequests.titleEditRegistrationRequest',
      },
      {
        path: 'admin',
        children: [
          {
            path: 'constituencies',
            component: Constituencies,
            canActivate: [PermissionGuard],
            title: 'constituencies.title',
            data: {
              permission: perm('accessConstituenciesAdminPage'),
            },
          },
          {
            path: 'constituencies/new',
            component: ConstituencyCreate,
            canActivate: [PermissionGuard],
            title: 'constituencies.titleNewConstituency',
            data: {
              permission: perm('createConstituency'),
            },
          },
          {
            path: 'constituencies/:id/edit',
            component: ConstituencyUpdate,
            canActivate: [PermissionGuard],
            title: 'constituencies.titleEditConstituency',
            data: {
              permission: perm('updateConstituency'),
            },
          },
          {
            path: 'polling-stations',
            component: PollingStations,
            canActivate: [PermissionGuard],
            title: 'pollingStations.title',
            data: {
              permission: perm('accessPollingStationsAdminPage'),
            },
          },
          {
            path: 'polling-stations/new',
            component: PollingStationCreate,
            canActivate: [PermissionGuard],
            title: 'pollingStations.titleNewPollingStation',
            data: {
              permission: perm('createPollingStation'),
            },
          },
          {
            path: 'polling-stations/:id/edit',
            component: PollingStationUpdate,
            canActivate: [PermissionGuard],
            title: 'pollingStations.titleEditPollingStation',
            data: {
              permission: perm('updatePollingStation'),
            },
          },
          {
            path: 'users',
            component: Users,
            canActivate: [PermissionGuard],
            title: 'users.title',
            data: {
              permission: perm('accessUsersAdminPage'),
            },
          },
          {
            path: 'users/new',
            component: UserCreate,
            canActivate: [PermissionGuard],
            title: 'users.titleNewUser',
            data: {
              permission: perm('createUser'),
            },
          },
          {
            path: 'users/:id/edit',
            component: UserUpdate,
            canActivate: [PermissionGuard],
            data: {
              permission: perm('updateUser'),
            },
            title: 'users.titleEditUser',
          },
        ],
      },
      {
        path: 'my-account',
        component: MyAccountUi,
        canActivate: [PermissionGuard],
        title: 'register.title',
      },
    ],
  },
  { path: '**', redirectTo: '/home' },
];
