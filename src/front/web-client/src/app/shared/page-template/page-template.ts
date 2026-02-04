import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Footer } from './footer/footer';
import { Navbar } from './navbar/navbar';

@Component({
  selector: 'app-page-template',
  imports: [Navbar, Footer, RouterOutlet],
  templateUrl: './page-template.html',
  styleUrl: './page-template.scss',
})
export class PageTemplate {}
