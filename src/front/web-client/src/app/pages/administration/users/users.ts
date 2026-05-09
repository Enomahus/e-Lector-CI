import { DatePipe, NgClass } from '@angular/common';
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
import { UsersApiService } from '@app/services/api/users.api.service';
import { PermissionDirective } from '@app/services/auth/permission.directive';
import { GetUsersQuery, GetUsersResponse } from '@app/services/nswag/api-nswag-client';
import { BaseTable } from '@app/shared/base-table/base-table';
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
  selector: 'app-users',
  imports: [
    PermissionDirective,
    TranslateModule,
    RouterLink,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatIconModule,
    NgClass,
    DatePipe,
  ],
  providers: [DatePipe],
  templateUrl: './users.html',
  styleUrl: './users.scss',
})
export class Users extends BaseTable<GetUsersResponse> {
  private readonly userService = inject(UsersApiService);
  private readonly translateService = inject(TranslateService);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);

  private currentSearch = signal('');
  private searchSubject = new BehaviorSubject<string>('');

  displayedColumns = signal<string[]>([
    'civility',
    'lastName',
    'firstName',
    'email',
    'phone',
    'constituency',
    'isActive',
    'isAdmin',
    'createdAt',
    'authProvider',
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
  ): Observable<{ data: GetUsersResponse[]; total: number }> {
    const query: GetUsersQuery = {
      sort,
      order,
      pageIndex: page,
      pageSize: this.paginator?.pageSize ?? 20,
      search: this.currentSearch(),
    };
    return this.userService.getUsers(query).pipe(
      map((result) => ({
        data: result.data?.items ?? [],
        total: result.data?.totalCount ?? 0,
      })),
      catchError(() => {
        return of({ data: [], total: 0 });
      }),
    );
  }

  onToggleActiveUser(user: GetUsersResponse): void {
    // Implement toggle active user logic here
  }

  onDeleteUser(user: GetUsersResponse): void {
    // Implement delete user logic here
  }

  onPageChange(event: PageEvent): void {
    if (this.paginator) {
      this.paginator.pageIndex = event.pageIndex;
    }
    this.refreshData();
  }
}
