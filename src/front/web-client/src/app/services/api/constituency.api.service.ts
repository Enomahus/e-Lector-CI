import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  CreateConstituencyCommand,
  GetConstituenciesQuery,
  GetConstituencyResponse,
  ResultOfGetConstituencyResponse,
  ResultOfLong,
  UpdateConstituencyCommandQuery,
} from '../nswag/api-nswag-client';
import { ApiBaseService } from './api-base.service';
import { ApiToastOptions } from './models/api-toast-options';

@Injectable({
  providedIn: 'root',
})
export class ConstituencyApiService extends ApiBaseService {
  createConstituency(
    command: CreateConstituencyCommand,
    options: ApiToastOptions = {},
  ): Observable<ResultOfLong> {
    return this.apiClient.createConstituency(command).pipe(this.handleResult(options));
  }
  updateConstituency(
    id: number,
    command: UpdateConstituencyCommandQuery,
    options: ApiToastOptions = {},
  ): Observable<ResultOfLong> {
    return this.apiClient.updateConstituency(id, command).pipe(this.handleResult(options));
  }

  getConstituency(
    id: number,
    options: ApiToastOptions = {},
  ): Observable<ResultOfGetConstituencyResponse> {
    return this.apiClient.getConstituency(id).pipe(this.handleResult(options));
  }

  getConstituencies(
    query: GetConstituenciesQuery,
    options: ApiToastOptions = {},
  ): Observable<GetConstituencyResponse[]> {
    return this.apiClient.getConstituencies(query).pipe(this.handleResult(options));
  }
}
