import { Routes } from '@angular/router';
import { Constituency } from './pages/administration/constituency/constituency';
import { PollingStation } from './pages/administration/polling-station/polling-station';
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
            path: 'polling-station',
            component: PollingStation,
            title: 'Polling station',
          },
        ],
      },
    ],
  },
];
