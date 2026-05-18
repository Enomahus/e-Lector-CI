import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  FileParameter,
  GetRegistrationRequestsQuery,
  RegistrationRequestModel,
  ResultOfGetRegistrationRequestResponse,
  ResultOfGuid,
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
    registrationRequestCertificateAttachments: FileParameter[] | undefined,
    registrationRequestCniAttachments: FileParameter[] | undefined,
    photo: FileParameter[] | undefined,
    options: ApiToastOptions = {},
  ): Observable<ResultOfGuid> {
    return this.apiClient
      .createRegistrationRequest(
        registrationRequestJson,
        registrationRequestCertificateAttachments,
        registrationRequestCniAttachments,
        photo,
      )
      .pipe(this.handleResult(options));
  }

  updateRegistrationRequest(
    id: string,
    registrationRequestJson: RegistrationRequestModel | undefined,
    registrationRequestCertificateAttachments: FileParameter[] | undefined,
    registrationRequestCniAttachments: FileParameter[] | undefined,
    photo: FileParameter[] | undefined,
    options: ApiToastOptions = {},
  ): Observable<ResultOfGuid> {
    return this.apiClient
      .updateRegistrationRequest(
        id,
        registrationRequestJson,
        registrationRequestCertificateAttachments,
        registrationRequestCniAttachments,
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
}
