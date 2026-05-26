import { Component } from '@angular/core';
import { AppPermission, GetRegistrationRequestsQuery } from '@app/services/nswag/api-nswag-client';
import {
  RegistrationRequestRow,
  RegistrationRequestsTableComponent,
  RegistrationRequestTableParams,
} from '@app/shared/registration-requests-table/registration-requests-table';
import { catchError, map, Observable, of } from 'rxjs';
import { AbstractRegistrationRequestsUI } from '../generic-registration-requests/abstract-registration-requests-ui';

/**
 * Standard user view – shows the current user's own registration requests.
 */
@Component({
  selector: 'app-registration-requests-ui',
  standalone: true,
  imports: [RegistrationRequestsTableComponent],
  templateUrl: '../generic-registration-requests/generic-registration-requests.html',
  styleUrls: ['../generic-registration-requests/generic-registration-requests.scss'],
})
export class RegistrationRequestsUi extends AbstractRegistrationRequestsUI {
  override readonly titleKey = 'registrationRequests.title';
  override readonly routePrefix = 'registration-requests';
  override readonly editPermission: AppPermission = 'updateRegistrationRequest';
  override readonly deletePermission: AppPermission = 'deleteRegistrationRequest';
  override readonly displayedColumns: string[] = [
    'reference',
    'submissionDate',
    'citizenName',
    'citizenBirthDate',
    'citizenBirthPlace',
    'status',
    'authorName',
    'constituencyName',
    'remark',
    'actions',
  ];

  protected override getData(
    params: RegistrationRequestTableParams,
  ): Observable<{ data: RegistrationRequestRow[]; total: number }> {
    const query: GetRegistrationRequestsQuery = {
      sort: params.sort,
      order: params.order,
      pageIndex: params.page,
      pageSize: params.pageSize,
      search: params.search,
    };
    return this.registrationRequestService.getRegistrationRequests(query).pipe(
      map((result) => ({
        data: (result.data?.items ?? []) as RegistrationRequestRow[],
        total: result.data?.totalCount ?? 0,
      })),
      catchError(() => of({ data: [], total: 0 })),
    );
  }

  protected override performDelete(item: RegistrationRequestRow): void {
    if (!item.id) return;
    // TODO: call registrationRequestService.deleteRegistrationRequest(item.id)
    //       when the endpoint is available in the API service.
    console.warn('deleteRegistrationRequest not yet implemented', item.id);
  }
}
