import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  FileResponse,
  GetDocumentsInfosQuery,
  ResultOfGetDocumentInfoResponse,
  ResultOfGetDocumentsInfosResponse,
} from '../nswag/api-nswag-client';
import { ApiBaseService } from './api-base.service';
import { ApiToastOptions } from './models/api-toast-options';

@Injectable({
  providedIn: 'root',
})
export class DocumentApiService extends ApiBaseService {
  getDocumentInfo(
    documentId: string,
    options: ApiToastOptions = {},
  ): Observable<ResultOfGetDocumentInfoResponse> {
    return this.apiClient.getDocumentInfo(documentId).pipe(this.handleResult(options));
  }

  getDocumentInfos(
    query: GetDocumentsInfosQuery,
    options: ApiToastOptions = {},
  ): Observable<ResultOfGetDocumentsInfosResponse> {
    return this.apiClient.getDocumentsInfos(query).pipe(this.handleResult(options));
  }

  downloadDocument(documentId: string, options: ApiToastOptions = {}): Observable<FileResponse> {
    return this.apiClient.downloadDocument(documentId).pipe(this.handleResult(options));
  }
}
