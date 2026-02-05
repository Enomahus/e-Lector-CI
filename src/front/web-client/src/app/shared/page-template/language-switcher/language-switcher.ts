import { AsyncPipe, CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { Observable } from 'rxjs';
import { Language } from '../../../enums/language.enum';
import { LanguageService } from '../../../services/language.service';

@Component({
  selector: 'app-language-switcher',
  imports: [CommonModule, TranslateModule, AsyncPipe],
  templateUrl: './language-switcher.html',
  styleUrl: './language-switcher.scss',
})
export class LanguageSwitcher {
  private readonly languageService = inject(LanguageService);
  langItems: { lang: Language; displayText: string }[];
  currentLang$: Observable<Language>;

  constructor() {
    // Voluntarily un-translated texts
    this.langItems = [
      { lang: Language.fr, displayText: 'Français' },
      { lang: Language.en, displayText: 'English' },
    ];
    this.currentLang$ = this.languageService.getCurrentLanguage();
  }

  langChange(lang: Language): void {
    this.languageService.changeLanguage(lang);
  }
}
