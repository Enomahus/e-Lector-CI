import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  CreatePollingStationCommand,
  ResultOfListOfGetPollingStationsByConstituencyIdResponse,
  ResultOfLong,
  ResultOfPollingStationModel,
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

  getPollingStationById(
    id: number,
    options: ApiToastOptions = {},
  ): Observable<ResultOfPollingStationModel> {
    return this.apiClient.getPollingStationById(id).pipe(this.handleResult(options));
  }

  getPollingStationsByConstituencyId(
    constituencyId: number,
    options: ApiToastOptions = {},
  ): Observable<ResultOfListOfGetPollingStationsByConstituencyIdResponse> {
    return this.apiClient
      .getPollingStationsByConstituencyId(constituencyId)
      .pipe(this.handleResult(options));
  }
}
