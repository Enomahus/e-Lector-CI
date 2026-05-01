import { CommonModule } from '@angular/common';
import { Component, computed, inject, resource, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { ConstituencyNode } from '@app/models/constituency.model';
import { ConstituencyApiService } from '@app/services/api/constituency.api.service';
import { GetConstituenciesResponse } from '@app/services/nswag/api-nswag-client';
import { ConstituencyTree } from '@app/shared/constituency-tree/constituency-tree';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-constituencies',
  imports: [TranslateModule, RouterLink, CommonModule, ConstituencyTree],
  templateUrl: './constituencies.html',
  styleUrl: './constituencies.scss',
})
export class Constituencies {
  private readonly translateService = inject(TranslateService);
  private readonly constituencyService = inject(ConstituencyApiService);
  private readonly router = inject(Router);

  isDeleting = signal(false);

  readonly constituenciesResource = resource({
    loader: async () => {
      const res = await firstValueFrom(this.constituencyService.getConstituencyTree({}));
      return res.data ?? [];
    },
  });

  readonly nodes = computed<ConstituencyNode[]>(() => {
    const data = this.constituenciesResource.value();
    return data ? data.map((c) => this.mapToNode(c)) : [];
  });

  readonly isLoading = this.constituenciesResource.isLoading;

  refresh(): void {
    this.constituenciesResource.reload();
  }

  onEdit(node: ConstituencyNode): void {
    this.router.navigate(['admin', 'constituencies', node.id, 'edit']);
  }
  onDelete(node: ConstituencyNode): void {
    if (node.id === undefined) return;

    this.isDeleting.set(true);

    this.constituencyService
      .deleteConstituency(node.id, {
        successMessage: this.translateService.instant('constituency.successDeleting'),
        errorMessage: this.translateService.instant('constituency.errorDeleting'),
      })
      .subscribe({
        next: () => {
          this.isDeleting.set(false);
          this.nodes();
        },
        error: () => {
          this.isDeleting.set(false);
        },
      });
  }

  onToggleStatus(node: ConstituencyNode): void {
    if (node.id === undefined) return;

    console.log('Toggle status for node:', node);
    // this.constituencyService
    //   .toggleConstituencyStatus(node.id, {
    //     successMessage: this.translateService.instant('constituency.successUpdating'),
    //     errorMessage: this.translateService.instant('constituency.errorUpdating'),
    //   })
    //   .subscribe(() => {
    //     this.nodes();
    //   });
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
