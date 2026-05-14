import { CommonModule } from '@angular/common';
import { Component, computed, inject, input, OnInit, output, signal } from '@angular/core';
import {
  FormControl,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Breadcrumbs } from '@app/models/breadcrumb.model';
import { ConstituencyNode } from '@app/models/constituency.model';
import { ConstituencyApiService } from '@app/services/api/constituency.api.service';
import { BreadcrumbService } from '@app/services/breadcrumb.service';
import {
  ConstituencyModel,
  GetConstituenciesResponse,
  GetConstituencyResponse,
  LocationLevel,
} from '@app/services/nswag/api-nswag-client';
import { ConstituencyTree } from '@app/shared/constituency-tree/constituency-tree';
import { StickyButtonsContainer } from '@app/shared/sticky-buttons-container/sticky-buttons-container';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-constituency',
  imports: [
    TranslateModule,
    CommonModule,
    ReactiveFormsModule,
    ConstituencyTree,
    StickyButtonsContainer,
  ],
  templateUrl: './constituency.html',
  styleUrl: './constituency.scss',
})
export class Constituency implements OnInit {
  private readonly constituecyService = inject(ConstituencyApiService);
  private readonly translateService = inject(TranslateService);
  private readonly breadcrumbService = inject(BreadcrumbService);

  constituency = input<GetConstituencyResponse | undefined>(undefined);
  constituencyId = input<number | undefined>(undefined);
  saveConstituency = output<ConstituencyModel>();
  goBack = output<void>();
  isSaving = input.required<boolean>();
  isToCreate = input.required<boolean>();

  nodes = signal<ConstituencyNode[]>([]);
  selectedNode = signal<ConstituencyNode | null>(null);
  initialTreeSelectedId = computed(() => (!this.isToCreate() ? this.constituencyId() : undefined));

  private fg = inject(NonNullableFormBuilder);
  parentConstituencies = signal<ConstituencyNode[]>([]);

  // Liste des niveaux pour le select
  levels: LocationLevel[] = [
    'region',
    'department',
    'subPrefecture',
    'municipality',
    'votingLocation',
  ];

  constituencyForm = this.fg.group({
    code: new FormControl<string>('', [Validators.required]),
    wording: new FormControl<string>('', [Validators.required]),
    level: new FormControl<LocationLevel>('votingLocation', { nonNullable: true }),
    parentId: new FormControl<number | undefined>(undefined, [Validators.required]),
    isActive: new FormControl<boolean>(true, [Validators.required]),
  });

  formData = signal<ConstituencyModel | null>(null);

  ngOnInit(): void {
    this.setBreadcrumbs(this.constituency());

    if (!this.isToCreate() && this.constituency()) {
      this.constituencyForm.patchValue(this.constituency()!);
    }
    this.constituecyService.getConstituencyTree({}).subscribe((res) => {
      const parent = res.data ?? [];
      const nodes = parent.map((c) => this.mapToNode(c));

      if (!this.isToCreate() && this.constituencyId()) {
        this.expandPathToNode(nodes, this.constituencyId()!);
      }

      this.nodes.set(nodes);

      if (!this.isToCreate() && this.constituency()?.parentId) {
        const parentNode = this.findNodeInTree(nodes, this.constituency()!.parentId!);
        if (parentNode) {
          this.parentConstituencies.set([parentNode]);
          this.selectedNode.set(parentNode);
        }
      }
    });
  }

  setBreadcrumbs(constituency: ConstituencyModel | undefined): void {
    let breadcrumbs: Breadcrumbs[] = [];
    breadcrumbs = [
      {
        label: this.translateService.instant('constituencies.title'),
        url: '/admin/constituencies',
      },
      {
        label: this.isToCreate()
          ? this.translateService.instant('constituency.newConstituencyTitle')
          : (constituency?.wording ?? ''),
      },
    ];
    this.breadcrumbService.setBreadcrumbs(breadcrumbs);
  }

  save(): void {
    if (this.constituencyForm.invalid) {
      this.constituencyForm.markAsTouched();
      return;
    }
    const rawValue = this.constituencyForm.getRawValue();
    this.formData.set(rawValue as ConstituencyModel);
    this.saveConstituency.emit(this.formData()!);
    this.setBreadcrumbs(this.formData()!);
  }

  onNodeSelected(info: ConstituencyNode): void {
    this.selectedNode.set(info);
    this.parentConstituencies.set([info]);
    this.constituencyForm.patchValue({ parentId: info.id, level: info.level, isActive: true });
    this.constituencyForm.get('parentId')?.markAsDirty();
    this.constituencyForm.get('isActive')?.markAsDirty();
  }

  private findNodeInTree(
    nodes: ConstituencyNode[],
    targetId: number,
  ): ConstituencyNode | undefined {
    for (const node of nodes) {
      if (node.id === targetId) return node;
      if (node.children?.length) {
        const found = this.findNodeInTree(node.children, targetId);
        if (found) return found;
      }
    }
    return undefined;
  }

  private expandPathToNode(nodes: ConstituencyNode[], targetId: number): boolean {
    for (const node of nodes) {
      if (node.id === targetId) return true;
      if (node.children?.length && this.expandPathToNode(node.children, targetId)) {
        node.expanded = true;
        return true;
      }
    }
    return false;
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
}
