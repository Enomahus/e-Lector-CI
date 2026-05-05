import {
  Component,
  DestroyRef,
  HostListener,
  inject,
  input,
  Input,
  output,
  signal,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router } from '@angular/router';
import { AuthService } from '@app/services/auth/auth.service';
import { filter, map, Observable, tap } from 'rxjs';
import { Language } from '../../../enums/language.enum';
import { LanguageService } from '../../../services/language.service';

@Component({
  standalone: true,
  template: '',
})
export abstract class BaseNavbar {
  @Input({ required: true }) userName$!: Observable<string>;
  isRegister = input.required<boolean>();
  logout = output<void>();

  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  private readonly languageService = inject(LanguageService);
  private readonly authService = inject(AuthService);

  dropdownOpen = signal(false);
  showAdminRequestsText = signal(false);
  showOrganismRequestsText = signal(false);
  showElectorRequestsText = signal(false);

  constructor() {
    // Hide dropdown after having navigated to another page.
    this.router.events
      .pipe(
        filter((e) => e instanceof NavigationEnd),
        tap(() => {
          this.dropdownOpen.set(false);
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe();

    this.authService
      .getPermissions()
      .pipe(
        map((permissions) => {
          if (permissions.includes('accessRegistrationRequestsForAdminPage')) {
            this.showAdminRequestsText.set(true);
          }
          // if(permissions.includes('accessRegistrationRequestsForOrganismPage')) {
          //   this.showOrganismRequestsText.set(true);
          // }
          // if(permissions.includes('accessRegistrationRequestsForElectorPage')) {
          //   this.showElectorRequestsText.set(true);
          // }
        }),
      )
      .subscribe();
  }

  isRouteMatching(route: string): boolean {
    return this.router.url.includes(route);
  }

  @HostListener('document:click', ['$event'])
  onClick(event: Event): void {
    // Hide dropdown after having clicked outside.
    const targetElement = event.target as HTMLElement;
    if (
      (!targetElement.closest('.dropdown-content') && !targetElement.closest('.dropbtn')) ||
      targetElement.offsetParent?.className === 'dropdown-content'
    ) {
      this.dropdownOpen.set(false);
    }
  }

  langChange(lang: Language): void {
    this.languageService.changeLanguage(lang);
  }

  goBackHome(): void {
    this.router.navigate(['/home']);
  }
}
