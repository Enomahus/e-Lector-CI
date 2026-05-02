import { AfterViewInit, Component, OnDestroy, signal, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort, SortDirection } from '@angular/material/sort';
import {
  BehaviorSubject,
  catchError,
  EMPTY,
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
  data = signal<TResponse[]>([]);
  resultsLength = signal(0);
  isLoadingResults = signal(true);

  protected destroy$ = new Subject<void>();
  protected refresh$ = new BehaviorSubject<void>(undefined); // Permet de déclencher un rafraîchissement manuel des données

  @ViewChild(MatPaginator) paginator?: MatPaginator;
  @ViewChild(MatSort) sort?: MatSort;

  abstract getData(
    sort: string,
    order: SortDirection,
    page: number,
    search?: string,
  ): Observable<{ data: TResponse[]; total: number }>;

  ngAfterViewInit(): void {
    if (!this.sort || !this.paginator) {
      return;
    }

    const sort = this.sort;
    const paginator = this.paginator;

    // Si l'utilisateur trie, on revient à la première page
    sort.sortChange.pipe(takeUntil(this.destroy$)).subscribe(() => (paginator.pageIndex = 0));

    merge(sort.sortChange, paginator.page, this.refresh$)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoadingResults.set(true);
          return this.getData(sort.active, sort.direction, paginator.pageIndex).pipe(
            catchError((err) => {
              if (err.name === 'CanceledError' || err.status === 0) {
                return EMPTY;
              }
              console.log('Error loading data', err);
              return of({ data: [], total: 0 });
            }),
          );
        }),
        map((response) => {
          this.isLoadingResults.set(false);

          if (!response) return this.data();

          this.resultsLength.set(response.total);
          return response.data;
        }),
        takeUntil(this.destroy$),
      )
      .subscribe((res) => this.data.set(res));
  }

  refreshData(): void {
    this.refresh$.next();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
