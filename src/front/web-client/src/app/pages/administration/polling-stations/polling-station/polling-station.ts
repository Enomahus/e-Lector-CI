import { CommonModule } from '@angular/common';
import {
  Component,
  computed,
  EventEmitter,
  inject,
  Input,
  OnInit,
  Output,
  signal,
} from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { Breadcrumbs } from '@app/models/breadcrumb.model';
import { ConstituencyNode } from '@app/models/constituency.model';
import { ConstituencyApiService } from '@app/services/api/constituency.api.service';
import { BreadcrumbService } from '@app/services/breadcrumb.service';
import {
  GetConstituenciesResponse,
  PollingStationModel,
} from '@app/services/nswag/api-nswag-client';

import { ConstituencyTree } from '@app/shared/constituency-tree/constituency-tree';
import { Loader } from '@app/shared/loader/loader';
import { StickyButtonsContainer } from '@app/shared/sticky-buttons-container/sticky-buttons-container';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import {
  createPollingStationForm,
  createPollingStationModelFromForm,
} from './polling-station-form';

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
export class PollingStation implements OnInit {
  private readonly translateService = inject(TranslateService);
  private readonly breadcrumbService = inject(BreadcrumbService);
  private readonly constituencyService = inject(ConstituencyApiService);

  @Output() save = new EventEmitter<PollingStationModel>();
  @Output() goBack = new EventEmitter<void>();
  @Input() station?: PollingStationModel;
  @Input() isToCreate = false;
  @Input() isSaving = false;

  nodes = signal<ConstituencyNode[]>([]);
  selectedNode = signal<ConstituencyNode | null>(null);

  isSubmitting = signal(false);

  form = createPollingStationForm();

  readonly canSubmit = computed(() => this.form.valid && !this.isSubmitting());

  ngOnInit(): void {
    this.setBreadcrumbs(this.station);
    this.loadConstituencyTree();

    if (!this.isToCreate && this.station) {
      this.form.patchValue(this.station);
    }
  }

  savePollingStation(): void {
    if (this.form.invalid) {
      this.form.markAsTouched();
      return;
    }
    this.isSubmitting.set(true);
    this.station = createPollingStationModelFromForm(this.form);
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

  private loadConstituencyTree(): void {
    this.constituencyService.getConstituencyTree({}).subscribe((response) => {
      const res = response.data;
      if (res) {
        const mappedNodes = res.map((c) => this.mapToNode(c));
        this.nodes.set(mappedNodes);

        if (this.station?.constituencyId) {
          const found = this.findNodeById(mappedNodes, this.station.constituencyId);
          if (found) {
            this.selectedNode.set(found);
          }
        }
      }
    });
  }

  private mapToNode(constituency: GetConstituenciesResponse): ConstituencyNode {
    return {
      id: constituency.id!,
      code: constituency.code!,
      wording: constituency.wording!,
      level: constituency.level!,
      parentId: constituency.parentId ?? undefined,
      children: constituency.children?.map((c) => this.mapToNode(c)),
      expanded: false,
    };
  }

  onNodeSelected(info: ConstituencyNode): void {
    const votingLocationNode = info.level === 'votingLocation' ? info : null;
    if (votingLocationNode) {
      this.selectedNode.set(votingLocationNode);
      this.form.patchValue({ constituencyId: votingLocationNode.id, isActive: true });
      this.form.get('constituencyId')?.markAsDirty();
      this.form.get('isActive')?.markAsDirty();
    }
  }

  private findNodeById(
    nodes: ConstituencyNode[],
    id: number | string,
  ): ConstituencyNode | undefined {
    for (const node of nodes) {
      if (node.id === id) return node;
      if (node.children) {
        const found = this.findNodeById(node.children, id);
        if (found) return found;
      }
    }
    return undefined;
  }
}
