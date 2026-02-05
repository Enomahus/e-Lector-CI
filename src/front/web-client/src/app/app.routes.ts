import { Routes } from '@angular/router';
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
        loadComponent: () => import('./pages/home/home').then((m) => m.Home),
        title: 'Home',
      },
      {
        path: 'admin',
        children: [
          {
            path: 'geographic-area',
            loadComponent: () => import('./pages/administration/geographic-area/geographic-area').then((m) => m.GeographicArea),
            title: 'Geographic area'
          },
          {
            path: 'polling-station',
            loadComponent: () => import('./pages/administration/polling-station/polling-station').then((m) => m.PollingStation),
            title: 'Polling station'
          }
        ]
      }
    ],
  },
];
