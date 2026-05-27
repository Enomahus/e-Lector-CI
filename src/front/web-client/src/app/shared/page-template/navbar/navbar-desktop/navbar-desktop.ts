import { AsyncPipe, CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { HideIfAdminDirective } from '@app/services/auth/hide-if-admin.directive';
import { PermissionDirective } from '@app/services/auth/permission.directive';
import { TranslateModule } from '@ngx-translate/core';
import { LanguageSwitcher } from '../../language-switcher/language-switcher';
import { BaseNavbar } from '../base-navbar';

@Component({
  selector: 'app-navbar-desktop',
  imports: [
    CommonModule,
    AsyncPipe,
    TranslateModule,
    RouterLink,
    RouterLinkActive,
    LanguageSwitcher,
    PermissionDirective,
    HideIfAdminDirective,
  ],
  templateUrl: './navbar-desktop.html',
  styleUrls: ['./navbar-desktop.scss'],
})
export class NavbarDesktop extends BaseNavbar {}
