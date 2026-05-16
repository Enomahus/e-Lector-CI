import { inject, Injectable, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ConstituencyNode } from '@app/models/constituency.model';
import { map } from 'rxjs';
import { ConstituencyApiService } from './api/constituency.api.service';
import { GetConstituenciesResponse } from './nswag/api-nswag-client';

@Injectable({
  providedIn: 'root',
})
export class ConstituencyTreeHelperService {
  private readonly constituencyService = inject(ConstituencyApiService);

  readonly nodesData = toSignal(
    this.constituencyService
      .getConstituencyTree({})
      .pipe(map((res) => res.data?.map((c) => this.mapToNode(c)) ?? [])),
    { initialValue: [] as ConstituencyNode[] },
  );

  // État privé (Signaux)
  //private readonly _nodes = signal<ConstituencyNode[]>([]);
  private readonly _selectedNode = signal<ConstituencyNode | null>(null);

  // Expositions publiques en lecture seule
  //readonly nodes = this._nodes.asReadonly();
  readonly selectedNode = this._selectedNode.asReadonly();

  expandNodePath(targetId: number): void {
    const currentNodes = this.nodesData();
    if (currentNodes.length > 0) {
      this.expandPathToNode(currentNodes, targetId);
    }
  }

  findNode(id: number): ConstituencyNode | undefined {
    return this.findNodeInTree(this.nodesData(), id);
  }

  setSelectedNode(node: ConstituencyNode | number | undefined): void {
    if (typeof node === 'number') {
      const foundNode = this.findNodeInTree(this.nodesData(), node);
      this._selectedNode.set(foundNode ?? null);
    } else {
      this._selectedNode.set(node ?? null);
    }
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
