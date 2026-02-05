import { Component } from '@angular/core';
import { BaseNavbar } from '../base-navbar';
import { TranslateModule } from '@ngx-translate/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { LanguageSwitcher } from '../../language-switcher/language-switcher';
import { AsyncPipe, CommonModule } from '@angular/common';

@Component({
  selector: 'app-navbar-desktop',
  imports: [CommonModule, AsyncPipe ,TranslateModule, RouterLink, RouterLinkActive, LanguageSwitcher],
  templateUrl: './navbar-desktop.html',
  styleUrl: './navbar-desktop.scss',
})
export class NavbarDesktop extends BaseNavbar {

}
