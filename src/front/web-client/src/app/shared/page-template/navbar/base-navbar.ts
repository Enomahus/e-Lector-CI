import {
  Component,
  computed,
  DestroyRef,
  HostListener,
  inject,
  input,
  output,
  signal,
} from '@angular/core';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router } from '@angular/router';
import { filter, map, Observable, tap } from 'rxjs';
import { Language } from '../../../enums/language.enum';
import { UsersApiService } from '../../../services/api/users.api.service';
import { AuthService } from '../../../services/auth/auth.service';
import { LanguageService } from '../../../services/language.service';
import { AppPermission, GetCurrentUserResponse } from '../../../services/nswag/api-nswag-client';

@Component({
  standalone: true,
  template: '',
})
export abstract class BaseNavbar {
  userName$ = input.required<Observable<string>>();
  isRegister = input.required<boolean>();
  logout = output<void>();

  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  private readonly languageService = inject(LanguageService);
  private readonly authService = inject(AuthService);
  private readonly userService = inject(UsersApiService);

  private readonly permissions = toSignal(this.authService.getPermissions(), {
    initialValue: [] as AppPermission[],
  });
  readonly currentUserRole = toSignal(
    this.userService
      .getCurrentUser()
      .pipe(map((user: GetCurrentUserResponse) => user.userRoleName || '')),
    {
      initialValue: null,
    },
  );

  readonly showAdminRequestsText = computed(() =>
    this.permissions().includes('accessRegistrationRequestsForAdminPage'),
  );
  readonly showOrganismRequestsText = computed(() =>
    this.permissions().includes('accessRegistrationRequestsForManagementPage'),
  );
  readonly showElectorRequestsText = computed(() =>
    this.permissions().includes('accessRegistrationRequestsPage'),
  );
  readonly isAdmin = computed(() => this.permissions().includes('superAdmin'));

  dropdownOpen = signal(false);

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
