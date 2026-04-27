import { NgClass } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { Router, RouterLink } from '@angular/router';
import { PollingStationApiService } from '@app/services/api/polling-station.api.service';
import { PermissionDirective } from '@app/services/auth/permission.directive';
import {
  GetPollingStationsQuery,
  GetPollingStationsResponse,
} from '@app/services/nswag/api-nswag-client';
import { BaseTable } from '@app/shared/base-table/base-table';
import { ConfirmDialog } from '@app/shared/confirm-dialog/confirm-dialog';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import {
  BehaviorSubject,
  catchError,
  debounceTime,
  distinctUntilChanged,
  map,
  Observable,
  of,
  takeUntil,
} from 'rxjs';

@Component({
  selector: 'app-polling-stations',
  imports: [
    TranslateModule,
    RouterLink,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatIconModule,
    PermissionDirective,
    NgClass,
  ],
  templateUrl: './polling-stations.html',
  styleUrl: './polling-stations.scss',
})
export class PollingStations extends BaseTable<GetPollingStationsResponse> {
  private readonly pollingStationService = inject(PollingStationApiService);
  private readonly translateService = inject(TranslateService);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);

  isDeleting = signal(false);
  private searchSubject = new BehaviorSubject<string>('');
  private currentSearch = signal('');

  displayedColumns = signal<string[]>([
    'stationNumber',
    'votingLocationName',
    'municipalityName',
    'subPrefectureName',
    'departmentName',
    'regionName',
    'actions',
  ]);

  constructor() {
    super();

    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe((value) => {
        this.currentSearch.set(value);
        if (this.paginator) {
          this.paginator.pageIndex = 0;
        }
        this.refreshData();
      });
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value.trim();
    this.searchSubject.next(filterValue);
  }

  override getData(
    sort: string,
    order: string,
    page: number,
  ): Observable<{ data: GetPollingStationsResponse[]; total: number }> {
    const query: GetPollingStationsQuery = {
      sort,
      order,
      pageIndex: page,
      pageSize: this.paginator?.pageSize ?? 20,
      search: this.currentSearch(),
    };
    return this.pollingStationService.getPollingStations(query).pipe(
      map((response) => ({
        data: response.data?.items ?? [],
        total: response.data?.totalCount ?? 0,
      })),
      catchError((err) => {
        if (err.name === 'CanceledError' || err.status === 0) {
          this.translateService.get('pollingStations.loadError').subscribe((msg) => alert(msg));
        }
        return of({ data: [], total: 0 });
      }),
    );
  }

  onEditPollingStation(row: GetPollingStationsResponse): void {
    this.router.navigate(['admin', 'polling-stations', row.stationId, 'edit']);
  }

  onDeletePollingStation(row: GetPollingStationsResponse): void {
    this.isDeleting.set(true);

    const dialogRef = this.dialog.open(ConfirmDialog, {
      width: '400px',
      data: { name: `le bureau de vote n°${row.stationNumber}` },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.pollingStationService.deletePollingStation(row.stationId ?? 0).subscribe({
          next: () => {
            this.isDeleting.set(false);
            this.refreshData();
          },
          error: () => {
            this.isDeleting.set(false);
          },
        });
      }
    });
  }

  onToggleActivePollingStation(row: GetPollingStationsResponse): void {
    const errorMessage = row.isDisabled
      ? this.translateService.instant('pollingStations.activateError')
      : this.translateService.instant('pollingStations.deactivateError');

    const successMessage = row.isDisabled
      ? this.translateService.instant('pollingStations.activated')
      : this.translateService.instant('pollingStations.deactivated');

    this.pollingStationService
      .togglePollingStationActive(
        { id: row.stationId ?? 0 },
        { successMessage: successMessage, errorMessage: errorMessage },
      )
      .subscribe({
        next: () => {
          this.refreshData();
        },
      });
  }

  onPageChange(event: PageEvent): void {
    this.paginator.pageIndex = event.pageIndex;
    this.refreshData();
  }
}
