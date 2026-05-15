import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, input, output, signal } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { ConstituencyNode } from '@app/models/constituency.model';
import { ConstituencyApiService } from '@app/services/api/constituency.api.service';
import { BreadcrumbService } from '@app/services/breadcrumb.service';
import { ConstituencyTreeHelperService } from '@app/services/constituency-tree-helper.service';
import { PollingStationModel } from '@app/services/nswag/api-nswag-client';

import { ConstituencyTree } from '@app/shared/constituency-tree/constituency-tree';
import { Loader } from '@app/shared/loader/loader';
import { StickyButtonsContainer } from '@app/shared/sticky-buttons-container/sticky-buttons-container';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { PollingStationForm } from './polling-station-form';

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
  styleUrl: './polling-station.scss',
})
export class PollingStation {
  private readonly translateService = inject(TranslateService);
  private readonly breadcrumbService = inject(BreadcrumbService);
  private readonly constituencyService = inject(ConstituencyApiService);
  private readonly store = inject(ConstituencyTreeHelperService);

  save = output<PollingStationModel>();
  goBack = output<void>();

  constituencyId = input<number | undefined>(undefined);
  isToCreate = input<boolean>(false);
  isSaving = input<boolean>(false);
  form = input.required<PollingStationForm>();

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
}
