import { Component } from '@angular/core';
import { Footer } from '../../../shared/page-template/footer/footer';
import { LanguageSwitcher } from '../../../shared/page-template/language-switcher/language-switcher';

@Component({
  selector: 'app-login-page-template',
  imports: [Footer, LanguageSwitcher],
  templateUrl: './login-page-template.html',
  styleUrl: './login-page-template.scss',
})
export class LoginPageTemplate {}
