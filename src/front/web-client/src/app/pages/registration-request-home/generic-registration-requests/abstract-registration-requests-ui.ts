import { Component, inject, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { GetRegistrationRequestsResponse } from '@app/services/nswag/api-nswag-client';
import { BaseTable } from '@app/shared/base-table/base-table';
import { TranslateService } from '@ngx-translate/core';
import { BehaviorSubject, debounceTime, distinctUntilChanged, takeUntil } from 'rxjs';

@Component({
  standalone: true,
  providers: [],
  template: '',
})
export abstract class AbstractRegistrationRequestsUI extends BaseTable<GetRegistrationRequestsResponse> {
  private readonly translateService = inject(TranslateService);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);

  isDeleting = signal(false);
  private searchSubject = new BehaviorSubject<string>('');
  private currentSearch = signal('');

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
}
