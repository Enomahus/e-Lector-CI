import { Component } from '@angular/core';
import {
  AppPermission,
  GetRegistrationRequestsForAdminQuery,
} from '@app/services/nswag/api-nswag-client';
import {
  RegistrationRequestRow,
  RegistrationRequestsTableComponent,
  RegistrationRequestTableParams,
} from '@app/shared/registration-requests-table/registration-requests-table';
import { catchError, map, Observable, of } from 'rxjs';
import { AbstractRegistrationRequestsUI } from '../generic-registration-requests/abstract-registration-requests-ui';

/**
 * Administrator view – full visibility over all registration requests with all
 * available columns and admin-level permissions.
 */
@Component({
  selector: 'app-registration-requests-for-admin-ui',
  standalone: true,
  imports: [RegistrationRequestsTableComponent],
  templateUrl: '../generic-registration-requests/generic-registration-requests.html',
  styleUrls: ['../generic-registration-requests/generic-registration-requests.scss'],
})
export class RegistrationRequestsForAdminUi extends AbstractRegistrationRequestsUI {
  override readonly titleKey = 'registrationRequests.titleForAdmin';
  override readonly routePrefix = 'registration-requests-for-admin';
  /** Admins manage requests via a dedicated permission. */
  override readonly editPermission: AppPermission = 'updateRegistrationRequest';
  /** No dedicated admin-delete permission exposed yet – directive shows the button unconditionally. */
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
    const query: GetRegistrationRequestsForAdminQuery = {
      sort: params.sort,
      order: params.order,
      pageIndex: params.page,
      pageSize: params.pageSize,
      search: params.search,
    };
    return this.registrationRequestService.getRegistrationRequestsForAdmin(query).pipe(
      map((result) => ({
        data: (result.data?.items ?? []) as RegistrationRequestRow[],
        total: result.data?.totalCount ?? 0,
      })),
      catchError(() => of({ data: [], total: 0 })),
    );
  }

  protected override performDelete(item: RegistrationRequestRow): void {
    if (!item.id) return;
    // TODO: call the admin delete endpoint when it is available in the API service.
    console.warn('deleteRegistrationRequest (admin) not yet implemented', item.id);
  }
}
