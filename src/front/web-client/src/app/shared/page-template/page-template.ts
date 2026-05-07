import { Component, inject, signal } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { Breadcrumb } from './breadcrumb/breadcrumb';
import { Footer } from './footer/footer';
import { Navbar } from './navbar/navbar';

@Component({
  selector: 'app-page-template',
  imports: [Navbar, Footer, RouterOutlet, Breadcrumb],
  templateUrl: './page-template.html',
  styleUrl: './page-template.scss',
})
export class PageTemplate {
  private readonly router = inject(Router);

  isRegisterRoute = signal(this.router.url === '/register');
}
