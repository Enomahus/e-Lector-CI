import { Component } from '@angular/core';
import { BaseNavbar } from '../base-navbar';
import { AsyncPipe, CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { LanguageSwitcher } from '../../language-switcher/language-switcher';

@Component({
  selector: 'app-navbar-mobile',
  imports: [CommonModule, AsyncPipe, TranslateModule, RouterLink, RouterLinkActive, LanguageSwitcher],
  templateUrl: './navbar-mobile.html',
  styleUrl: './navbar-mobile.scss',
})
export class NavbarMobile extends BaseNavbar {}
