import { AfterViewInit, Component, OnDestroy, signal, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort, SortDirection } from '@angular/material/sort';
import {
  catchError,
  map,
  merge,
  Observable,
  of,
  startWith,
  Subject,
  switchMap,
  takeUntil,
} from 'rxjs';

@Component({
  selector: 'app-base-table',
  template: '',
})
export abstract class BaseTable<TResponse> implements AfterViewInit, OnDestroy {
  // Signaux pour l'état de l'UI
  data = signal<TResponse[]>([]);
  resultsLength = signal(0);
  isLoadingResults = signal(true);

  protected destroy$ = new Subject<void>();

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  abstract getData(
    sort: string,
    order: SortDirection,
    page: number,
  ): Observable<{ data: TResponse[]; total: number }>;

  ngAfterViewInit(): void {
    // Si l'utilisateur trie, on revient à la première page
    this.sort.sortChange
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => (this.paginator.pageIndex = 0));

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoadingResults.set(true);
          return this.getData(this.sort.active, this.sort.direction, this.paginator.pageIndex).pipe(
            catchError(() => of(null)),
          );
        }),
        map((response) => {
          this.isLoadingResults.set(false);

          if (response === null) return [];

          this.resultsLength.set(response.total);
          return response.data;
        }),
        takeUntil(this.destroy$),
      )
      .subscribe((res) => this.data.set(res));
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
