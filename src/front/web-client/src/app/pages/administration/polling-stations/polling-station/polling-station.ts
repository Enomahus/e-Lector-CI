import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, input, output, signal } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { ConstituencyNode } from '@app/models/constituency.model';
import { BreadcrumbService } from '@app/services/breadcrumb.service';
import { ConstituencyTreeHelperService } from '@app/services/constituency-tree-helper.service';
import {
  GetPollingStationResponse,
  PollingStationModel,
} from '@app/services/nswag/api-nswag-client';

import { Breadcrumbs } from '@app/models/breadcrumb.model';
import { ConstituencyTree } from '@app/shared/constituency-tree/constituency-tree';
import { Loader } from '@app/shared/loader/loader';
import { StickyButtonsContainer } from '@app/shared/sticky-buttons-container/sticky-buttons-container';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { createPollingStationForm, PollingStationForm } from './polling-station-form';

@Component({
  selector: 'app-polling-station',
  imports: [
    TranslateModule,
    StickyButtonsContainer,
    Loader,
    CommonModule,
    ReactiveFormsModule,
    ConstituencyTree,
  ],
  templateUrl: './polling-station.html',
  styleUrls: ['./polling-station.scss'],
})
export class PollingStation {
  private readonly translateService = inject(TranslateService);
  private readonly breadcrumbService = inject(BreadcrumbService);
  private readonly store = inject(ConstituencyTreeHelperService);

  save = output<PollingStationModel>();
  goBack = output<void>();
  constituencyId = input<number | undefined>(undefined);
  pollingStation = input<GetPollingStationResponse | undefined>(undefined);
  isToCreate = input<boolean>(false);
  isSaving = input<boolean>(false);

  form = signal<PollingStationForm>(createPollingStationForm(this.isToCreate()));
  nodes = this.store.nodesData;
  selectedNode = this.store.selectedNode;
  initialTreeSelectedId = computed(() => (!this.isToCreate() ? this.constituencyId() : undefined));

  isSubmitting = signal(false);

  readonly canSubmit = computed(() => this.form().valid && !this.isSubmitting());

  constructor() {
    effect(() => {
      const nodes = this.nodes();
      const id = this.constituencyId();

      if (nodes.length > 0 && id) {
        this.store.expandNodePath(id);
        this.store.setSelectedNode(id);
      }
    });

    effect(() => {
      const station = this.pollingStation();
      if (station) {
        this.form().patchValue({
          stationNumber: station.stationNumber,
          wording: station.wording,
          constituencyId: station.constituencyId,
          isActive: station.isActive,
        });
        this.setBreadcrumb(station);
      }
    });
  }

  savePollingStation(): void {
    if (this.form().invalid) {
      this.form().markAsTouched();
      return;
    }
    this.isSubmitting.set(true);
    const updateModel = this.form().getRawValue() as PollingStationModel;
    this.save.emit(updateModel);
  }

  onNodeSelected(info: ConstituencyNode): void {
    const votingLocationNode = info.level === 'votingLocation' ? info : null;
    if (votingLocationNode) {
      this.store.setSelectedNode(votingLocationNode);
      this.form().patchValue({ constituencyId: votingLocationNode.id, isActive: true });
      this.form().get('constituencyId')?.markAsDirty();
      this.form().get('isActive')?.markAsDirty();
    }
  }

  private setBreadcrumb(station: GetPollingStationResponse): void {
    let breadcrumbs: Breadcrumbs[] = [];

    breadcrumbs = [
      {
        label: this.translateService.instant('breadcrumb.pollingStations'),
        url: `/admin/polling-stations`,
      },
      {
        label: this.isToCreate()
          ? this.translateService.instant('breadcrumb.pollingStationCreate')
          : `${station.wording} ${station.stationNumber}`,
      },
    ];
    this.breadcrumbService.setBreadcrumbs(breadcrumbs);
  }
}
