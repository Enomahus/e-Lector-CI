import { Routes } from '@angular/router';
import { Constituency } from './pages/administration/constituency/constituency';
import { PollingStationCreate } from './pages/administration/polling-stations/polling-station-create/polling-station-create';
import { PollingStationUpdate } from './pages/administration/polling-stations/polling-station-update/polling-station-update';
import { PollingStations } from './pages/administration/polling-stations/polling-stations';
import { Home } from './pages/home/home';
import { PageTemplate } from './shared/page-template/page-template';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/login',
    pathMatch: 'full',
  },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login').then((m) => m.Login),
    title: 'Login',
  },
  {
    path: 'forgot-password',
    loadComponent: () =>
      import('./pages/forgot-password/forgot-password').then((m) => m.ForgotPassword),
    title: 'Forgot Password',
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./pages/login/create-account/create-account').then((m) => m.CreateAccount),
    title: 'Create an account',
  },
  {
    path: '',
    component: PageTemplate,
    children: [
      {
        path: 'home',
        component: Home,
        title: 'Home',
      },
      {
        path: 'admin',
        children: [
          {
            path: 'constituency',
            component: Constituency,
            title: 'Constituency',
          },
          {
            path: 'polling-stations',
            component: PollingStations,
            title: 'Polling Stations',
          },
          {
            path: 'polling-stations/new',
            component: PollingStationCreate,
            title: 'Create Polling Station',
          },
          {
            path: 'polling-stations/:id/edit',
            component: PollingStationUpdate,
            title: 'Update Polling Station',
          },
        ],
      },
    ],
  },
];
