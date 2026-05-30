import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  GetDashboardStatsQuery,
  ResultOfGetDashboardStatsResponse,
} from '../nswag/api-nswag-client';
import { ApiBaseService } from './api-base.service';
import { ApiToastOptions } from './models/api-toast-options';

@Injectable({
  providedIn: 'root',
})
export class DashboardApiService extends ApiBaseService {
  getDashboardStats(
    query: GetDashboardStatsQuery,
    options: ApiToastOptions = {},
  ): Observable<ResultOfGetDashboardStatsResponse> {
    return this.apiClient.getDashboardStats(query).pipe(this.handleResult(options));
  }
}
