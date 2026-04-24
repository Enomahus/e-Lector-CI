import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  CreatePollingStationCommand,
  GetPollingStationsQuery,
  Result,
  ResultOfLong,
  ResultOfPagedListOfGetPollingStationsResponse,
  ResultOfPollingStationModel,
  ToogleActivePollingStationCommand,
  UpdatePollingStationCommand,
} from '../nswag/api-nswag-client';
import { ApiBaseService } from './api-base.service';
import { ApiToastOptions } from './models/api-toast-options';

@Injectable({
  providedIn: 'root',
})
export class PollingStationApiService extends ApiBaseService {
  createPollingStation(
    command: CreatePollingStationCommand,
    options: ApiToastOptions = {},
  ): Observable<ResultOfLong> {
    return this.apiClient.createPollingStation(command).pipe(this.handleResult(options));
  }

  updatePollingStation(
    id: number,
    command: UpdatePollingStationCommand,
    options: ApiToastOptions = {},
  ): Observable<ResultOfLong> {
    return this.apiClient.updatePollingStation(id, command).pipe(this.handleResult(options));
  }

  deletePollingStation(id: number, options: ApiToastOptions = {}): Observable<Result> {
    return this.apiClient.deletePollingStation(id).pipe(this.handleResult(options));
  }

  togglePollingStationActive(
    command: ToogleActivePollingStationCommand,
    options: ApiToastOptions = {},
  ): Observable<Result> {
    return this.apiClient.toogleActivePollingStation(command).pipe(this.handleResult(options));
  }

  getPollingStationById(
    id: number,
    options: ApiToastOptions = {},
  ): Observable<ResultOfPollingStationModel> {
    return this.apiClient.getPollingStationById(id).pipe(this.handleResult(options));
  }

  getPollingStations(
    command: GetPollingStationsQuery,
    options: ApiToastOptions = {},
  ): Observable<ResultOfPagedListOfGetPollingStationsResponse> {
    return this.apiClient.getPollingStations(command).pipe(this.handleResult(options));
  }
}
