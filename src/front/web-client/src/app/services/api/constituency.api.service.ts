import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  CreateConstituencyCommand,
  GetConstituenciesQuery,
  Result,
  ResultOfGetConstituencyResponse,
  ResultOfIEnumerableOfGetConstituenciesResponse,
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

  getConstituencyTree(
    query: GetConstituenciesQuery,
    options: ApiToastOptions = {},
  ): Observable<ResultOfIEnumerableOfGetConstituenciesResponse> {
    return this.apiClient.getConstituencies(query).pipe(this.handleResult(options));
  }

  deleteConstituency(id: number, options: ApiToastOptions = {}): Observable<Result> {
    return this.apiClient.deleteConstituency(id).pipe(this.handleResult(options));
  }
}
