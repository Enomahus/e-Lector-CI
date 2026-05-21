import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ResetPasswordCommand, Result } from '../nswag/api-nswag-client';
import { ApiBaseService } from './api-base.service';
import { ApiToastOptions } from './models/api-toast-options';

@Injectable({
  providedIn: 'root',
})
export class SecurityService extends ApiBaseService {
  forgotPassword(email: string, options: ApiToastOptions = {}): Observable<Result> {
    return this.apiClient.forgotPassword(email).pipe(this.handleResult(options));
  }

  resetPassword(command: ResetPasswordCommand, options: ApiToastOptions = {}): Observable<Result> {
    return this.apiClient.resetPassword(command).pipe(this.handleResult(options));
  }
}
