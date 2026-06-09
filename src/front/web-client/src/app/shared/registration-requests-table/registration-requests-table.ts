import { DatePipe, NgClass } from '@angular/common';
import { AfterViewInit, Component, input, OnDestroy, output, ViewChild } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';
import { PermissionDirective } from '@app/services/auth/permission.directive';
import {
  AppPermission,
  GetRegistrationRequestsResponseModel,
  RegistrationStatus,
} from '@app/services/nswag/api-nswag-client';
import { TranslateModule } from '@ngx-translate/core';
import {
  debounceTime,
  delay,
  distinctUntilChanged,
  merge,
  startWith,
  Subject,
  takeUntil,
} from 'rxjs';

/** Params emitted on each user interaction (sort, page, search). */
export interface RegistrationRequestTableParams {
  sort: string;
  order: string;
  page: number;
  pageSize: number;
  search: string;
}

/**
 * Extended row type: all three response types share the base model; management
 * and admin responses additionally carry `authorName`.
 */
export type RegistrationRequestRow = GetRegistrationRequestsResponseModel;
//authorName?: string;

/**
 * Dumb / presentational component.
 *
 * Renders a full Angular-Material table (sort, pagination, search) for
 * registration-request data.  It knows nothing about the API – the parent smart
 * component feeds `data` / `resultsLength` / `isLoading` via signal inputs and
 * reacts to the `paramsChange` output to trigger fresh API calls.
 */
@Component({
  selector: 'app-registration-requests-table',
  standalone: true,
  imports: [
    DatePipe,
    ReactiveFormsModule,
    RouterLink,
    TranslateModule,
    PermissionDirective,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatChipsModule,
    NgClass,
  ],
  templateUrl: './registration-requests-table.html',
  styleUrl: './registration-requests-table.scss',
})
export class RegistrationRequestsTableComponent implements AfterViewInit, OnDestroy {
  readonly data = input<RegistrationRequestRow[]>([]);
  readonly resultsLength = input(0);
  readonly isLoading = input(false);
  readonly displayedColumns = input<string[]>([
    'reference',
    'submissionDate',
    'citizenName',
    'status',
    'constituencyName',
    'actions',
  ]);
  readonly titleKey = input('registrationRequests.title');
  /** Route prefix used to build the edit link: `/${editRoutePrefix()}/${row.id}`. */
  readonly editRoutePrefix = input<string | undefined>(undefined);
  /** Permission required to show the edit button (undefined = always visible). */
  readonly editPermission = input<AppPermission | undefined>(undefined);
  /** Permission required to show the delete button (undefined = always visible). */
  readonly deletePermission = input<AppPermission | undefined>(undefined);
  /** Full router link for the "create new" button. Omit to hide the button. */
  readonly createRouteLink = input<string | undefined>(undefined);
  /** Permission required to show the create button (undefined = always visible). */
  readonly createPermission = input<AppPermission | undefined>(undefined);

  readonly requestDelete = output<RegistrationRequestRow>();
  readonly paramsChange = output<RegistrationRequestTableParams>();

  // ─── ViewChildren ───────────────────────────────────────────────────────────

  @ViewChild(MatSort) private readonly sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  readonly searchControl = new FormControl('', { nonNullable: true });
  private readonly destroy$ = new Subject<void>();

  ngAfterViewInit(): void {
    // Reset to first page whenever the user changes sort direction / column.
    this.sort.sortChange
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => (this.paginator.pageIndex = 0));

    // Debounce free-text search and reset to first page on each new value.
    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe(() => {
        this.paginator.pageIndex = 0;
        this.emitCurrentParams();
      });

    // Emit on sort / page changes – and once immediately on init (startWith).
    merge(this.sort.sortChange, this.paginator.page)
      .pipe(startWith({}), delay(0), takeUntil(this.destroy$))
      .subscribe(() => this.emitCurrentParams());
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private emitCurrentParams(): void {
    this.paramsChange.emit({
      sort: this.sort.active ?? '',
      order: this.sort.direction ?? '',
      page: this.paginator.pageIndex,
      pageSize: this.paginator.pageSize,
      search: this.searchControl.value,
    });
  }

  private readonly statusColorMap: Record<string, string> = {
    pending: 'status-registration-pending',
    tobeprocessed: 'status-registration-to-be-processed',
    approved: 'status-registration-approved',
    rejected: 'status-registration-rejected',
  };

  getRegistrationStatusClass(status: RegistrationStatus | string | undefined): string {
    if (!status) return 'status-registration-no-status';

    const normalizedStatus = String(status).toLowerCase();
    return this.statusColorMap[normalizedStatus] || 'status-registration-no-status';
  }
}
