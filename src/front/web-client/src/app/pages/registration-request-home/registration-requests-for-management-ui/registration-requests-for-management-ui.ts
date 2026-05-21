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
 * Management (organism) view – shows all requests visible to the manager,
 * including the author column and management-specific actions.
 */
@Component({
  selector: 'app-registration-requests-for-management-ui',
  standalone: true,
  imports: [RegistrationRequestsTableComponent],
  templateUrl: '../generic-registration-requests/generic-registration-requests.html',
  styleUrls: ['../generic-registration-requests/generic-registration-requests.scss'],
})
export class RegistrationRequestsForManagementUi extends AbstractRegistrationRequestsUI {
  override readonly titleKey = 'registrationRequests.titleForManagement';
  override readonly routePrefix = 'registration-requests-for-management';
  override readonly editPermission: AppPermission = 'updateRegistrationRequestsForManagement';
  override readonly deletePermission: AppPermission = 'deleteRegistrationRequestsForManagement';
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
    return this.registrationRequestService.getRegistrationRequestsForManagement(query).pipe(
      map((result) => ({
        data: (result.data?.items ?? []) as RegistrationRequestRow[],
        total: result.data?.totalCount ?? 0,
      })),
      catchError(() => of({ data: [], total: 0 })),
    );
  }

  protected override performDelete(item: RegistrationRequestRow): void {
    if (!item.id) return;
    // TODO: call registrationRequestService.deleteRegistrationRequestForManagement(item.id)
    //       when the endpoint is available in the API service.
    console.warn('deleteRegistrationRequestForManagement not yet implemented', item.id);
  }
}
