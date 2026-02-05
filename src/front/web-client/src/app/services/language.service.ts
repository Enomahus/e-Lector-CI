import { inject, Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { Language, Locale } from '../enums/language.enum';

@Injectable({
  providedIn: 'root',
})
export class LanguageService {
  private readonly translateService = inject(TranslateService);
   private readonly langStorageKey = 'app_lang';
  private readonly currentLang = new BehaviorSubject<Language>(Language.en);

   constructor() {
    this.translateService.use(Language.en);
    const storedLanguage: string | null = localStorage.getItem(this.langStorageKey);
    if (storedLanguage) {
      this.changeLanguage(this.getLanguageFromString(storedLanguage));
    } else {
      this.useBrowserLanguage();
    }
  }

  useBrowserLanguage(): void {
    const browserLocale = navigator.languages?.length ? navigator.languages[0] : navigator.language;
    const lang = this.getLanguageFromString(browserLocale);
    this.changeLanguage(lang);
  }

  private getLanguageFromString(langStr: string): Language {
    let lang = Language.en;
    if (langStr.startsWith('fr')) {
      lang = Language.fr;
    }
    return lang;
  }

  changeLanguage(lang: Language): void {
    localStorage.setItem(this.langStorageKey, lang);
    this.translateService.use(lang);
    this.currentLang.next(lang);
  }

  getCurrentLanguage(): Observable<Language> {
    return this.currentLang.asObservable();
  }

  getCurrentLocale(): Observable<Locale> {
    return this.currentLang.pipe(
      map((lang) => {
        switch (lang) {
          case Language.fr:
            return Locale.fr;
          case Language.en:
            return Locale.en;
        }
      }),
    );
  }

  // readonly STOCKAGE_KEY = 'app_lang';
  // private readonly availableLangs = ['fr', 'en'];

  // constructor() {
  //   const stored = localStorage.getItem(this.STOCKAGE_KEY);
  //   const browser = navigator.language.split('-')[0];
  //   const fallback = 'fr';

  //   const initial =
  //     stored && this.availableLangs.includes(stored)
  //       ? stored
  //       : this.availableLangs.includes(browser)
  //         ? browser
  //         : fallback;

  //   this.translateService.addLangs(this.availableLangs);
  //   this.translateService.setFallbackLang(fallback);
  //   this.translateService.use(initial);
  // }

  // get currentLang(): string {
  //   return this.translateService.getCurrentLang() || this.translateService.getFallbackLang()!;
  // }

  // get languages(): string[] {
  //   return this.availableLangs;
  // }

  // setLanguage(lang: string): void {
  //   if (!this.availableLangs.includes(lang)) return;

  //   this.translateService.use(lang);
  //   localStorage.setItem(this.STOCKAGE_KEY, lang);
  // }
}
