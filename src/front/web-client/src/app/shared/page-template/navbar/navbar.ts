import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router } from '@angular/router';
import { AuthService } from '@app/services/auth/auth.service';
import { TranslateModule } from '@ngx-translate/core';
import { filter, Observable, tap } from 'rxjs';
import { CurrentUserService } from '../../../services/current-user.service';
import { NavbarDesktop } from './navbar-desktop/navbar-desktop';
import { NavbarMobile } from './navbar-mobile/navbar-mobile';

@Component({
  selector: 'app-navbar',
  imports: [TranslateModule, NavbarDesktop, NavbarMobile],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss',
})
export class Navbar {
  private readonly destroyRef = inject(DestroyRef);
  private readonly router = inject(Router);
  private readonly currentUserService = inject(CurrentUserService);
  private readonly authService = inject(AuthService);

  isToggleMobileMenu = signal(false);
  userName$: Observable<string>;

  constructor() {
    this.userName$ = this.currentUserService.currentUserName$.pipe(
      takeUntilDestroyed(this.destroyRef),
    );
    this.router.events.pipe(
      filter((e) => e instanceof NavigationEnd),
      tap(() => {
        this.isToggleMobileMenu.set(false);
      }),
      takeUntilDestroyed(this.destroyRef),
    );
  }

  toggleMobileMenu(): void {
    this.isToggleMobileMenu.set(!this.isToggleMobileMenu);
  }

  handleLogout(): void {
    this.authService.logout();
  }
}
