import { HttpEvent, HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '@app/services/auth/auth.service';
import { APP_BASE_URL } from '@app/services/nswag/api-nswag-client';
import { Observable, switchMap } from 'rxjs';

export function AuthInterceptor(
  req: HttpRequest<unknown>,
  next: HttpHandlerFn,
): Observable<HttpEvent<unknown>> {
  const baseApiUrl = inject(APP_BASE_URL);
  if (!req.url.startsWith(baseApiUrl)) {
    return next(req);
  }

  let pathName = req.url;

  if (req.url.startsWith('http')) {
    const url = new URL(req.url);
    pathName = url.pathname;
  }

  // No bearer token added for anonymous routes
  if (pathName.startsWith('/auth') || pathName.startsWith('/user/register')) {
    return next(req);
  }

  return inject(AuthService)
    .getAccessToken()
    .pipe(
      switchMap((authToken) => {
        if (authToken) {
          const headers = req.headers.append('Authorization', `Bearer ${authToken}`);
          req = req.clone({
            headers,
          });
        }

        return next(req);
      }),
    );
}
