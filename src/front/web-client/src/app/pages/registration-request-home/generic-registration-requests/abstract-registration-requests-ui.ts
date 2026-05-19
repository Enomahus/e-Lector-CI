import { Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatDialog } from '@angular/material/dialog';
import { RegistrationRequestApiService } from '@app/services/api/registration-request.api.service';
import { AppPermission } from '@app/services/nswag/api-nswag-client';
import { ConfirmDialog } from '@app/shared/confirm-dialog/confirm-dialog';
import {
  RegistrationRequestRow,
  RegistrationRequestTableParams,
} from '@app/shared/registration-requests-table/registration-requests-table';
import { catchError, Observable, of, Subject, switchMap } from 'rxjs';

/**
 * Smart abstract base for all registration-request list views.
 *
 * Responsibilities:
 *  - Manages server-side data loading via a `switchMap` pipeline (cancels
 *    in-flight requests automatically).
 *  - Exposes `data`, `resultsLength`, and `isLoading` signals consumed by
 *    the shared `generic-registration-requests` template.
 *  - Delegates display logic entirely to the dumb
 *    `RegistrationRequestsTableComponent`.
 *
 * Concrete sub-classes must provide:
 *  - `titleKey`, `routePrefix`, `editPermission`, `deletePermission`,
 *    `displayedColumns` – static configuration.
 *  - `getData(params)` – API call returning the paged result.
 *  - (Optional) `performDelete(item)` – actual delete API call.
 */
@Component({
  standalone: true,
  template: '',
})
export abstract class AbstractRegistrationRequestsUI {
  // ─── DI ─────────────────────────────────────────────────────────────────────

  protected readonly registrationRequestService = inject(RegistrationRequestApiService);
  protected readonly dialog = inject(MatDialog);

  // ─── Abstract configuration – override in concrete classes ──────────────────

  abstract readonly titleKey: string;
  abstract readonly routePrefix: string;
  abstract readonly editPermission: AppPermission | undefined;
  abstract readonly deletePermission: AppPermission | undefined;
  abstract readonly displayedColumns: string[];

  // ─── State signals ───────────────────────────────────────────────────────────

  readonly data = signal<RegistrationRequestRow[]>([]);
  readonly resultsLength = signal(0);
  readonly isLoading = signal(false);
  readonly isDeleting = signal(false);

  // ─── Internal reactive pipeline ─────────────────────────────────────────────

  private readonly params$ = new Subject<RegistrationRequestTableParams>();

  constructor() {
    this.params$
      .pipe(
        switchMap((params) => {
          this.isLoading.set(true);
          return this.getData(params).pipe(
            catchError(() => of({ data: [] as RegistrationRequestRow[], total: 0 })),
          );
        }),
        takeUntilDestroyed(),
      )
      .subscribe((result) => {
        this.data.set(result.data);
        this.resultsLength.set(result.total);
        this.isLoading.set(false);
      });
  }

  // ─── Abstract data method ────────────────────────────────────────────────────

  protected abstract getData(
    params: RegistrationRequestTableParams,
  ): Observable<{ data: RegistrationRequestRow[]; total: number }>;

  // ─── Public event handlers (bound in the shared template) ───────────────────

  /** Called by the table component whenever sort, page, or search changes. */
  onParamsChange(params: RegistrationRequestTableParams): void {
    this.params$.next(params);
  }

  /** Opens a confirmation dialog then delegates to `performDelete`. */
  onDelete(item: RegistrationRequestRow): void {
    const name = item.reference ?? item.id ?? '';
    const dialogRef = this.dialog.open(ConfirmDialog, { data: { name } });

    dialogRef.afterClosed().subscribe((confirmed: boolean) => {
      if (confirmed) {
        this.performDelete(item);
      }
    });
  }

  // ─── Protected hooks ─────────────────────────────────────────────────────────

  /**
   * Override in concrete sub-classes to call the delete API.
   * Default is a no-op (feature not yet available for all views).
   */
  protected performDelete(_item: RegistrationRequestRow): void {
    // Implement in concrete class when the delete endpoint is wired up.
  }
}
