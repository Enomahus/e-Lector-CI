import { inject, Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  GuardResult,
  MaybeAsync,
  Router,
} from '@angular/router';
import { map, switchMap, tap } from 'rxjs';
import { AppPermission } from '../nswag/api-nswag-client';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class PermissionGuard implements CanActivate {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  canActivate(route: ActivatedRouteSnapshot): MaybeAsync<GuardResult> {
    const requiredPermissions = route.data['permissions'] as AppPermission | undefined;
    const canActivate$ = this.authService.isAuthenticated().pipe(
      tap((isAuth) => {
        if (!isAuth) {
          this.router.navigate(['/login'], { queryParams: { state: route.url.join('/') } });
        }
      }),
      map(() => true),
    );

    if (!requiredPermissions) {
      return canActivate$;
    }

    return canActivate$.pipe(
      switchMap(() => this.authService.getPermissions()),
      map((userPermissions) => userPermissions.includes(requiredPermissions)),
      tap((hasPermission) => {
        if (!hasPermission) {
          this.router.navigate(['/home']);
        }
      }),
    );
  }
}
