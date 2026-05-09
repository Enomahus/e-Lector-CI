import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Breadcrumbs } from '@app/models/breadcrumb.model';
import { PollingStationApiService } from '@app/services/api/polling-station.api.service';
import { BreadcrumbService } from '@app/services/breadcrumb.service';
import { PollingStationModel } from '@app/services/nswag/api-nswag-client';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { PollingStation } from '../polling-station/polling-station';
import {
  createPollingStationForm,
  PollingStationForm,
} from '../polling-station/polling-station-form';

@Component({
  selector: 'app-polling-station-create',
  imports: [TranslateModule, PollingStation],
  templateUrl: './polling-station-create.html',
  styleUrl: './polling-station-create.scss',
})
export class PollingStationCreate implements OnInit {
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly pollingStationService = inject(PollingStationApiService);
  private readonly translateService = inject(TranslateService);
  private readonly breadcrumbService = inject(BreadcrumbService);

  isSaving = signal(false);
  form = signal<PollingStationForm>(createPollingStationForm(true));
  constituencyId = signal<number | undefined>(undefined);

  ngOnInit(): void {
    const constituencyId = this.route.snapshot.queryParamMap.get('constituencyId');
    if (constituencyId) {
      this.constituencyId.set(Number(constituencyId));
    }
    this.setBreadcrumb();
  }

  private setBreadcrumb(): void {
    let breadcrumbs: Breadcrumbs[] = [];
    breadcrumbs = [
      {
        label: this.translateService.instant('breadcrumb.pollingStations'),
        url: `/admin/polling-stations`,
      },
      {
        label: this.translateService.instant('breadcrumb.pollingStationCreate'),
      },
    ];
    this.breadcrumbService.setBreadcrumbs(breadcrumbs);
  }

  submittedForm(station: PollingStationModel): void {
    this.isSaving.set(true);
    this.pollingStationService
      .createPollingStation(station, {
        errorMessage: this.translateService.instant('pollingStation.errorCreating'),
        successMessage: this.translateService.instant('pollingStation.successCreating'),
      })
      .subscribe({
        next: () => {
          this.isSaving.set(false);
          this.goBack();
        },
        error: () => {
          this.isSaving.set(false);
        },
      });
  }

  goBack() {
    this.router.navigate(['/admin/polling-stations']);
  }
}
