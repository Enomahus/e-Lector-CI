import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PollingStationApiService } from '@app/services/api/polling-station.api.service';
import { PollingStationModel } from '@app/services/nswag/api-nswag-client';
import { Loader } from '@app/shared/loader/loader';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { PollingStation } from '../polling-station/polling-station';

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

  isLoading = signal(false);
  isSaving = signal(false);
  pollingStation = signal<PollingStationModel | undefined>(undefined);
  pollingStationId = signal<number | undefined>(undefined);

  ngOnInit(): void {
    const stationId = Number(this.route.snapshot.paramMap.get('id'));
    if (stationId) {
      this.pollingStationId.set(stationId);
      this.isLoading.set(true);
      this.pollingSationService.getPollingStationById(stationId).subscribe({
        next: (response) => {
          this.pollingStation.set(response.data);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
          this.router.navigate(['admin', 'polling-stations']);
        },
      });
    }
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
