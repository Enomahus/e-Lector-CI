import { Component } from '@angular/core';
import { BaseNavbar } from '../base-navbar';
import { TranslateModule } from '@ngx-translate/core';
import { RouterLink } from '@angular/router';
import { LanguageSwitcher } from '../../language-switcher/language-switcher';

@Component({
  selector: 'app-navbar-desktop',
  imports: [TranslateModule, RouterLink, LanguageSwitcher],
  templateUrl: './navbar-desktop.html',
  styleUrl: './navbar-desktop.scss',
})
export class NavbarDesktop extends BaseNavbar {

}
