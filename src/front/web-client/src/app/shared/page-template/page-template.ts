import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Breadcrumb } from './breadcrumb/breadcrumb';
import { Footer } from './footer/footer';
import { Navbar } from './navbar/navbar';

@Component({
  selector: 'app-page-template',
  imports: [Navbar, Footer, RouterOutlet, Breadcrumb],
  templateUrl: './page-template.html',
  styleUrl: './page-template.scss',
})
export class PageTemplate {}
