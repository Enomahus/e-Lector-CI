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
import { Router } from '@angular/router';
import { Breadcrumbs } from '@app/models/breadcrumb.model';
import { ConstituencyNode } from '@app/models/constituency.model';
import { ConstituencyApiService } from '@app/services/api/constituency.api.service';
import { BreadcrumbService } from '@app/services/breadcrumb.service';
import {
  GetConstituenciesResponse,
  PollingStationModel,
} from '@app/services/nswag/api-nswag-client';
import { ConstituencyTree } from '@app/shared/constituency-tree';
import { Loader } from '@app/shared/loader/loader';
import { StickyButtonsContainer } from '@app/shared/sticky-buttons-container/sticky-buttons-container';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import {
  createPollingStationForm,
  createPollingStationModelFromForm,
} from './polling-station-form';

const createInitialState = (): PollingStationModel => ({
  stationNumber: '',
  wording: '',
  constituencyId: undefined,
  isActive: true,
});

@Component({
  selector: 'app-polling-station',
  imports: [
    ConstituencyTree,
    TranslateModule,
    StickyButtonsContainer,
    Loader,
    CommonModule,
    ReactiveFormsModule,
  ],
  templateUrl: './polling-station.html',
  styleUrl: './polling-station.scss',
})
export class PollingStation implements OnInit {
  private readonly router = inject(Router);
  private readonly translateService = inject(TranslateService);
  private readonly breadcrumbService = inject(BreadcrumbService);
  private readonly constituencyService = inject(ConstituencyApiService);

  @Output() save = new EventEmitter<PollingStationModel>();
  @Output() goBack = new EventEmitter<void>();
  @Input() station?: PollingStationModel;
  @Input() isToCreate = false;
  @Input() isSaving = false;

  readonly nodes = signal<ConstituencyNode[]>([]);

  model = signal<PollingStationModel>(createInitialState());
  readonly isSubmitting = signal(false);

  form = createPollingStationForm();

  readonly canSubmit = computed(() => this.form.valid && !this.isSubmitting());

  ngOnInit(): void {
    this.setBreadcrumbs(this.station);
    if (!this.isToCreate && this.station) {
      this.form.patchValue({
        stationNumber: this.station.stationNumber,
        wording: this.station.wording,
        constituencyId: this.station.constituencyId,
        isActive: this.station.isActive,
      });
    }

    this.loadConstituencyTree();
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

  cancel(): void {
    this.goBack.emit();
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
        this.nodes.set(res.map((c) => this.mapToNode(c)));
      }
    });
  }

  private mapToNode(constituency: GetConstituenciesResponse): ConstituencyNode {
    return {
      id: constituency.id!,
      code: constituency.code!,
      wording: constituency.wording!,
      level: constituency.level!,
      children: constituency.children?.map((c) => this.mapToNode(c)),
      expanded: false,
    };
  }

  onNodeSelected(node: ConstituencyNode | null): void {
    if (node) {
      this.form.value.constituencyId = node.id;
    }
  }
}
