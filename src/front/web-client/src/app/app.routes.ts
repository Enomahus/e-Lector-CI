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
import { PermissionGuard } from './services/auth/permission.guard';
import { PageTemplate } from './shared/page-template/page-template';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/login',
    pathMatch: 'full',
  },
  {
    path: 'login',
    component: Login,
    title: 'Login',
  },
  {
    path: 'forgot-password',
    component: ForgotPassword,
    title: 'Forgot Password',
  },
  {
    path: '',
    component: PageTemplate,
    children: [
      {
        path: 'home',
        component: Home,
        canActivate: [PermissionGuard],
        title: 'Home',
      },
      {
        path: 'register',
        component: CreateAccount,
        title: 'Create an account',
      },
      {
        path: 'registration-requests',
        component: RegistrationRequestsUi,
        canActivate: [PermissionGuard],
        title: 'Registration Requests',
      },
      {
        path: 'registration-requests-for-management',
        component: RegistrationRequestsForManagementUi,
        canActivate: [PermissionGuard],
        title: 'Registration Requests for Management',
      },
      {
        path: 'registration-requests-for-admin',
        component: RegistrationRequestsForAdminUi,
        canActivate: [PermissionGuard],
        title: 'Registration Requests for Admin',
      },
      {
        path: 'registration-requests/new',
        component: CreateRegistrationRequestUi,
        canActivate: [PermissionGuard],
        data: {
          requiredPermission: 'createRegistrationRequest',
        },
        title: 'Create Registration Request',
      },
      {
        path: 'registration-requests/:id',
        component: UpdateRegistrationRequestUi,
        canActivate: [PermissionGuard],
        data: {
          requiredPermission: 'updateRegistrationRequest',
        },
        title: 'Update Registration Request',
      },
      {
        path: 'admin',
        children: [
          {
            path: 'constituencies',
            component: Constituencies,
            canActivate: [PermissionGuard],
            title: 'Constituencies',
          },
          {
            path: 'constituencies/new',
            component: ConstituencyCreate,
            canActivate: [PermissionGuard],
            title: 'Create constituency',
          },
          {
            path: 'constituencies/:id/edit',
            component: ConstituencyUpdate,
            canActivate: [PermissionGuard],
            title: 'Update constituency',
          },
          {
            path: 'polling-stations',
            component: PollingStations,
            canActivate: [PermissionGuard],
            title: 'Polling Stations',
          },
          {
            path: 'polling-stations/new',
            component: PollingStationCreate,
            canActivate: [PermissionGuard],
            title: 'Create Polling Station',
          },
          {
            path: 'polling-stations/:id/edit',
            component: PollingStationUpdate,
            canActivate: [PermissionGuard],
            title: 'Update Polling Station',
          },
          {
            path: 'users',
            component: Users,
            canActivate: [PermissionGuard],
            title: 'Users',
          },
          {
            path: 'users/new',
            component: UserCreate,
            canActivate: [PermissionGuard],
            title: 'Create User',
          },
          {
            path: 'users/:id/edit',
            component: UserUpdate,
            canActivate: [PermissionGuard],
            title: 'Update User',
          },
        ],
      },
      {
        path: 'my-account',
        component: MyAccountUi,
        canActivate: [PermissionGuard],
        title: 'My Account',
      },
    ],
  },
];
