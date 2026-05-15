import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, input, OnInit, output, signal } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { Breadcrumbs } from '@app/models/breadcrumb.model';
import { ConstituencyNode } from '@app/models/constituency.model';
import { allLocationLevel } from '@app/pages/types/enumerations';
import { BreadcrumbService } from '@app/services/breadcrumb.service';
import { ConstituencyTreeHelperService } from '@app/services/constituency-tree-helper.service';
import {
  ConstituencyModel,
  GetConstituencyResponse,
  LocationLevel,
} from '@app/services/nswag/api-nswag-client';
import { ConstituencyTree } from '@app/shared/constituency-tree/constituency-tree';
import { StickyButtonsContainer } from '@app/shared/sticky-buttons-container/sticky-buttons-container';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ConstituencyForm, createConstituencyForm } from './constituency-form';

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
  private readonly translateService = inject(TranslateService);
  private readonly breadcrumbService = inject(BreadcrumbService);
  private readonly store = inject(ConstituencyTreeHelperService);

  constituency = input<GetConstituencyResponse | undefined>(undefined);
  constituencyId = input<number | undefined>(undefined);
  saveConstituency = output<ConstituencyModel>();
  goBack = output<void>();
  isToCreate = input.required<boolean>();
  isSaving = input.required<boolean>();
  form = signal<ConstituencyForm>(createConstituencyForm());

  nodes = this.store.nodesData;
  selectedNode = this.store.selectedNode;
  initialTreeSelectedId = computed(() => (!this.isToCreate() ? this.constituencyId() : undefined));

  parentConstituencies = signal<ConstituencyNode[]>([]);

  // Liste des niveaux pour le select
  levels: LocationLevel[] = allLocationLevel;

  formData = signal<ConstituencyModel | null>(null);

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

  ngOnInit(): void {
    if (this.constituency()) {
      this.form().patchValue({
        code: this.constituency()?.code,
        wording: this.constituency()?.wording,
        level: this.constituency()?.level,
        parentId: this.constituency()?.parentId,
        isActive: this.constituency()?.isActive,
      });
      this.setBreadcrumbs(this.constituency());
    }
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
    if (this.form().invalid) {
      this.form().markAsTouched();
      return;
    }
    const rawValue = this.form().getRawValue();
    this.formData.set(rawValue as ConstituencyModel);
    this.saveConstituency.emit(this.formData()!);
  }

  onNodeSelected(info: ConstituencyNode): void {
    this.store.setSelectedNode(info);
    this.parentConstituencies.set([info]);
    this.form().patchValue({ parentId: info.id, level: info.level, isActive: true });
    this.form().get('parentId')?.markAsDirty();
    this.form().get('isActive')?.markAsDirty();
  }
}
