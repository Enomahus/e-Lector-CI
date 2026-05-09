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
  //pollingStation = input<PollingStationModel | undefined>(undefined);
  constituencyId = input<number | undefined>(undefined);
  isToCreate = input<boolean>(false);
  isSaving = input<boolean>(false);
  form = input.required<PollingStationForm>();
  // private fg = inject(NonNullableFormBuilder);

  nodes = this.store.nodesData;
  selectedNode = this.store.selectedNode;
  //selectedNodes = signal<ConstituencyNode[]>([]);
  initialTreeSelectedId = computed(() => (!this.isToCreate() ? this.constituencyId() : undefined));

  isSubmitting = signal(false);

  // form = this.fg.group({
  //   stationNumber: new FormControl<string>('', {
  //     validators: Validators.required,
  //     nonNullable: true,
  //   }),
  //   wording: new FormControl<string>('', { validators: Validators.required, nonNullable: true }),
  //   constituencyId: new FormControl<number | undefined>(undefined, {
  //     validators: Validators.required,
  //     nonNullable: true,
  //   }),
  //   isActive: new FormControl<boolean>(false, {
  //     validators: Validators.required,
  //     nonNullable: true,
  //   }),
  // });

  //formData = signal<PollingStationModel | null>(null);

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

  // ngOnInit(): void {
  //   this.setBreadcrumbs(this.pollingStation());

  //   // if (!this.isToCreate() && this.pollingStation()) {
  //   //   this.form.patchValue(this.pollingStation()!);
  //   // }
  //   // this.loadConstituencyTree();
  // }

  savePollingStation(): void {
    if (this.form().invalid) {
      this.form().markAsTouched();
      return;
    }
    this.isSubmitting.set(true);
    const updateModel = this.form().getRawValue() as PollingStationModel;
    //this.formData.set(updateModel);
    this.save.emit(updateModel);
  }

  // setBreadcrumbs(station: PollingStationModel | undefined): void {
  //   let breadcrumbs: Breadcrumbs[] = [];

  //   breadcrumbs = [
  //     ...breadcrumbs,
  //     {
  //       label: this.translateService.instant('pollingStations.title'),
  //       url: '/admin/polling-stations',
  //     },
  //     {
  //       label: this.isToCreate()
  //         ? this.translateService.instant('pollingStation.newStationTitle')
  //         : (station?.wording ?? ''),
  //     },
  //   ];
  //   this.breadcrumbService.setBreadcrumbs(breadcrumbs);
  // }

  // private loadConstituencyTree(): void {
  //   this.constituencyService.getConstituencyTree({}).subscribe((response) => {
  //     const res = response.data ?? [];

  //     const nodes = res.map((c) => this.mapToNode(c));
  //     if (!this.isToCreate() && this.pollingStation()?.constituencyId) {
  //       this.expendPathToNode(nodes, this.pollingStation()!.constituencyId!);
  //     }
  //     this.nodes.set(nodes);

  //     if (!this.isToCreate() && this.pollingStation()?.constituencyId) {
  //       const parentNode = this.findNodeById(nodes, this.pollingStation()!.constituencyId!);
  //       if (parentNode) {
  //         this.selectedNodes.set([parentNode]);
  //         this.selectedNode.set(parentNode);
  //       }
  //     }
  //   });
  // }

  onNodeSelected(info: ConstituencyNode): void {
    const votingLocationNode = info.level === 'votingLocation' ? info : null;
    if (votingLocationNode) {
      //this.selectedNode.set(votingLocationNode);
      this.store.setSelectedNode(votingLocationNode);
      this.form().patchValue({ constituencyId: votingLocationNode.id, isActive: true });
      this.form().get('constituencyId')?.markAsDirty();
      this.form().get('isActive')?.markAsDirty();
    }
  }

  // private findNodeById(
  //   nodes: ConstituencyNode[],
  //   id: number | string,
  // ): ConstituencyNode | undefined {
  //   for (const node of nodes) {
  //     if (node.id === id) return node;
  //     if (node.children) {
  //       const found = this.findNodeById(node.children, id);
  //       if (found) return found;
  //     }
  //   }
  //   return undefined;
  // }

  // private expendPathToNode(nodes: ConstituencyNode[], targetId: number): boolean {
  //   for (const node of nodes) {
  //     if (node.id === targetId) return true;
  //     if (node.children?.length && this.expendPathToNode(node.children, targetId)) {
  //       node.expanded = true;
  //       return true;
  //     }
  //   }
  //   return false;
  // }

  // private mapToNode(constituency: GetConstituenciesResponse): ConstituencyNode {
  //   return {
  //     id: constituency.id!,
  //     code: constituency.code!,
  //     wording: constituency.wording!,
  //     level: constituency.level!,
  //     parentId: constituency.parentId ?? undefined,
  //     children: constituency.children?.map((c) => this.mapToNode(c)),
  //     expanded: false,
  //   };
  // }
}
