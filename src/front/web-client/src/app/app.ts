import { AsyncPipe } from '@angular/common';
import { Component, Renderer2, signal, ViewContainerRef } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { map, Observable } from 'rxjs';
import { ThemeService } from './services/theme.service';
import { Loader } from './shared/loader/loader';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Loader, AsyncPipe],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly title = signal('web-client');
  currentTheme?: string;
  translationsLoaded$: Observable<boolean>;

  constructor(
    translate: TranslateService,
    themeService: ThemeService,
    renderer: Renderer2,
    public viewRef: ViewContainerRef,
  ) {
    translate.addLangs(['fr']);
    translate.use('fr');
    this.translationsLoaded$ = translate.use('fr').pipe(
      // Map to true when translations are loaded
      // You can add more logic here if needed
      // For simplicity, we just return true
      // In a real app, you might want to check if translations are actually loaded
      // or handle errors
      map(() => true),
    );

    themeService.currentTheme$.subscribe((theme) => {
      if (this.currentTheme) {
        renderer.removeClass(document.body, this.currentTheme);
      }
      this.currentTheme = theme.toString();
      renderer.addClass(document.body, this.currentTheme);
    });
  }
}
