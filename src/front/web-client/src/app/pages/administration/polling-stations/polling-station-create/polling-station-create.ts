import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { PollingStationApiService } from '@app/services/api/polling-station.api.service';
import { PollingStationModel } from '@app/services/nswag/api-nswag-client';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { PollingStation } from '../polling-station/polling-station';

@Component({
  selector: 'app-polling-station-create',
  imports: [TranslateModule, PollingStation],
  templateUrl: './polling-station-create.html',
  styleUrl: './polling-station-create.scss',
})
export class PollingStationCreate {
  private readonly router = inject(Router);
  private readonly pollingStationService = inject(PollingStationApiService);
  private readonly translateService = inject(TranslateService);

  isSaving = signal(false);

  savePollingStation(station: PollingStationModel): void {
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
