import { AsyncPipe, CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { HideIfAdminDirective } from '../../../../services/auth/hide-if-admin.directive';
import { PermissionDirective } from '../../../../services/auth/permission.directive';
import { LanguageSwitcher } from '../../language-switcher/language-switcher';
import { BaseNavbar } from '../base-navbar';

@Component({
  selector: 'app-navbar-mobile',
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
  templateUrl: './navbar-mobile.html',
  styleUrls: ['./navbar-mobile.scss'],
})
export class NavbarMobile extends BaseNavbar {}
