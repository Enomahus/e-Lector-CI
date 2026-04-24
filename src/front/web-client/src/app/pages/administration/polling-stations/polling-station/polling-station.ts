import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { Breadcrumbs } from '@app/models/breadcrumb.model';
import { BreadcrumbService } from '@app/services/breadcrumb.service';
import { PollingStationModel } from '@app/services/nswag/api-nswag-client';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-polling-station',
  imports: [],
  templateUrl: './polling-station.html',
  styleUrl: './polling-station.scss',
})
export class PollingStation implements OnInit {
  private readonly router = inject(Router);
  private readonly translateService = inject(TranslateService);
  private readonly breadcrumbService = inject(BreadcrumbService);

  @Output() save = new EventEmitter<PollingStationModel>();
  @Output() goBack = new EventEmitter<void>();
  @Input() station?: PollingStationModel;
  @Input() isToCreate = false;
  @Input() isSaving = false;

  ngOnInit(): void {
    this.setBreadcrumbs(this.station);
    if (!this.isToCreate) {
    }
  }

  savePollingStation(): void {
    this.save.emit(this.station);
    this.setBreadcrumbs(this.station);
  }

  setBreadcrumbs(station: PollingStationModel | undefined): void {
    let breadcrumbs: Breadcrumbs[] = [];

    breadcrumbs = [
      ...breadcrumbs,
      {
        label: this.translateService.instant('pollingStations.title'),
        url: '/admin/polling-stations',
      },
      {
        label: this.isToCreate
          ? this.translateService.instant('pollingStation.newStationTitle')
          : (station!.wording ?? ''),
      },
    ];
    this.breadcrumbService.setBreadcrumbs(breadcrumbs);
  }
}
