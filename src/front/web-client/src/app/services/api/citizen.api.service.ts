import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ResultOfListOfGetCitizensResponse } from '../nswag/api-nswag-client';
import { ApiBaseService } from './api-base.service';
import { ApiToastOptions } from './models/api-toast-options';

@Injectable({
  providedIn: 'root',
})
export class CitizenApiService extends ApiBaseService {
  getCitizens(options: ApiToastOptions = {}): Observable<ResultOfListOfGetCitizensResponse> {
    return this.apiClient.getCitizens().pipe(this.handleResult(options));
  }
}
