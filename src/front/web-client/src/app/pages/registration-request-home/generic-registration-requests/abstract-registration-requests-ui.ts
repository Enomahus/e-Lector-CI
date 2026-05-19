import { Component, inject, signal } from '@angular/core';
import { GetRegistrationRequestsResponse } from '@app/services/nswag/api-nswag-client';
import { BaseTable } from '@app/shared/base-table/base-table';
import { TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';

@Component({
  standalone: true,
  providers: [],
  template: '',
})
export abstract class AbstractRegistrationRequestsUI<
  TResponse extends GetRegistrationRequestsResponse,
> extends BaseTable<TResponse> {
  private readonly translateService = inject(TranslateService);

  protected abstract titleKey: string;
  protected abstract routePrefix: string;

  isDeleting = signal(false);

  constructor() {
    super();
  }

  protected abstract override getData(
    sort: string,
    order: string,
    page: number,
  ): Observable<{
    data: TResponse[];
    total: number;
  }>;

  protected getYesOrNo(bool: boolean | undefined): string {
    return bool
      ? this.translateService.instant('global.yes')
      : this.translateService.instant('global.no');
  }

  protected getStatusLabel(status: string | string[]): string {
    return this.translateValue(status, 'registrationRequest.status');
  }

  private translateValue(value: string | string[], prefix: string): string {
    if (!value) return '';

    const values = Array.isArray(value) ? value : [value];
    return values
      .map((v) => this.translateService.instant(`${prefix}.${v.toLowerCase()}`))
      .join(', ');
  }
}
