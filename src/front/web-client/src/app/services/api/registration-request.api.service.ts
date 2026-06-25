import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  FileParameter,
  GetRegistrationRequestsForAdminQuery,
  GetRegistrationRequestsQuery,
  RegistrationRequestModel,
  ResultOfGetRegistrationRequestResponse,
  ResultOfGuid,
  ResultOfPagedListOfGetRegistrationRequestsForAdminResponse,
  ResultOfPagedListOfGetRegistrationRequestsForManagementResponse,
  ResultOfPagedListOfGetRegistrationRequestsResponse,
  ResultOfUpdateRegistrationRequestStatusResponse,
  UpdateRegistrationRequestStatusCommand,
} from '../nswag/api-nswag-client';
import { ApiBaseService } from './api-base.service';
import { ApiToastOptions } from './models/api-toast-options';

@Injectable({
  providedIn: 'root',
})
export class RegistrationRequestApiService extends ApiBaseService {
  createRegistrationRequest(
    registrationRequestJson: RegistrationRequestModel | undefined,
    registrationRequestCniOrCertificateAttachments: FileParameter | undefined,
    photo: FileParameter | undefined,
    options: ApiToastOptions = {},
  ): Observable<ResultOfGuid> {
    return this.apiClient
      .createRegistrationRequest(
        registrationRequestJson,
        registrationRequestCniOrCertificateAttachments,
        photo,
      )
      .pipe(this.handleResult(options));
  }

  updateRegistrationRequest(
    id: string,
    registrationRequestJson: RegistrationRequestModel | undefined,
    registrationRequestCniOrCertificateAttachments: FileParameter | undefined,
    photo: FileParameter | undefined,
    options: ApiToastOptions = {},
  ): Observable<ResultOfGuid> {
    return this.apiClient
      .updateRegistrationRequest(
        id,
        registrationRequestJson,
        registrationRequestCniOrCertificateAttachments,
        photo,
      )
      .pipe(this.handleResult(options));
  }

  updateRegistrationRequestStatus(
    id: string,
    command: UpdateRegistrationRequestStatusCommand,
    options: ApiToastOptions = {},
  ): Observable<ResultOfUpdateRegistrationRequestStatusResponse> {
    return this.apiClient
      .updateRegistrationRequestStatus(id, command)
      .pipe(this.handleResult(options));
  }

  getRegistrationRequest(
    id: string,
    options: ApiToastOptions = {},
  ): Observable<ResultOfGetRegistrationRequestResponse> {
    return this.apiClient.getRegistrationRequest(id).pipe(this.handleResult(options));
  }

  getRegistrationRequests(
    query: GetRegistrationRequestsQuery,
    options: ApiToastOptions = {},
  ): Observable<ResultOfPagedListOfGetRegistrationRequestsResponse> {
    return this.apiClient.getRegistrationRequests(query).pipe(this.handleResult(options));
  }

  getRegistrationRequestsForAdmin(
    query: GetRegistrationRequestsForAdminQuery,
    options: ApiToastOptions = {},
  ): Observable<ResultOfPagedListOfGetRegistrationRequestsForAdminResponse> {
    return this.apiClient.getRegistrationRequestsForAdmin(query).pipe(this.handleResult(options));
  }

  getRegistrationRequestsForManagement(
    query: GetRegistrationRequestsForAdminQuery,
    options: ApiToastOptions = {},
  ): Observable<ResultOfPagedListOfGetRegistrationRequestsForManagementResponse> {
    return this.apiClient
      .getRegistrationRequestsForManagement(query)
      .pipe(this.handleResult(options));
  }
}
