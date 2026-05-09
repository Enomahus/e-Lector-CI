import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { Breadcrumbs } from '@app/models/breadcrumb.model';
import { PollingStationApiService } from '@app/services/api/polling-station.api.service';
import { BreadcrumbService } from '@app/services/breadcrumb.service';
import {
  GetPollingStationResponse,
  PollingStationModel,
} from '@app/services/nswag/api-nswag-client';
import { Loader } from '@app/shared/loader/loader';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { map, switchMap, tap } from 'rxjs';
import { PollingStation } from '../polling-station/polling-station';
import {
  createPollingStationForm,
  PollingStationForm,
} from '../polling-station/polling-station-form';

@Component({
  selector: 'app-polling-station-update',
  imports: [TranslateModule, PollingStation, Loader],
  templateUrl: './polling-station-update.html',
  styleUrl: './polling-station-update.scss',
})
export class PollingStationUpdate implements OnInit {
  private readonly pollingSationService = inject(PollingStationApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly translateService = inject(TranslateService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly breadcrumbService = inject(BreadcrumbService);

  form = signal<PollingStationForm>(createPollingStationForm(false));
  isLoading = signal(false);
  isSaving = signal(false);
  constituencyId = signal<number | undefined>(undefined);
  pollingStation = signal<GetPollingStationResponse | undefined>(undefined);
  pollingStationId = signal<number | undefined>(undefined);

  ngOnInit(): void {
    this.route.params
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        map((param) => param['id']),
        tap((id) => this.pollingStationId.set(id)),
        switchMap((id) => this.pollingSationService.getPollingStationById(id)),
        tap((station) => {
          this.pollingStation.set(station.data);
          this.constituencyId.set(station.data?.constituencyId);
          this.loadStaion(station.data!);
        }),
      )
      .subscribe();

    this.setBreadcrumb();
  }

  private loadStaion(station: GetPollingStationResponse): void {
    this.form().patchValue({
      stationNumber: station.stationNumber,
      wording: station.wording,
      constituencyId: station.constituencyId,
      isActive: station.isActive,
    });
  }

  private setBreadcrumb(): void {
    let breadcrumbs: Breadcrumbs[] = [];

    breadcrumbs = [
      {
        label: this.translateService.instant('breadcrumb.pollingStations'),
        url: `/admin/polling-stations`,
      },
      {
        label: this.translateService.instant('breadcrumb.pollingStationEdit'),
      },
    ];
    this.breadcrumbService.setBreadcrumbs(breadcrumbs);
  }

  updatePollingStation(model: PollingStationModel): void {
    if (!this.pollingStationId()) {
      return;
    }
    this.isSaving.set(true);
    this.pollingSationService
      .updatePollingStation(this.pollingStationId()!, model, {
        successMessage: this.translateService.instant('pollingStation.successUpdating'),
        errorMessage: this.translateService.instant('pollingStation.errorUpdating'),
      })
      .subscribe({
        next: () => {
          this.isSaving.set(false);
        },
        error: () => {
          this.isSaving.set(false);
        },
      });
  }

  navigateToPollingStations(): void {
    this.router.navigate(['/admin/polling-stations']);
  }
}
