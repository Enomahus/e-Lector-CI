import { Tree, TreeItem, TreeItemGroup } from '@angular/aria/tree';
import { NgTemplateOutlet } from '@angular/common';
import { Component, input, output, signal } from '@angular/core';
import { ConstituencyNode } from '@app/models/constituency.model';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-constituency-tree',
  imports: [Tree, TreeItem, TreeItemGroup, NgTemplateOutlet, TranslateModule],
  templateUrl: './constituency-tree.html',
  styleUrl: './constituency-tree.scss',
})
export class ConstituencyTree {
  // Inputs utilisant les Signals (Angular 17.2+)
  nodes = input.required<ConstituencyNode[]>();
  // Output réactif
  nodeSelected = output<ConstituencyNode>();

  // État interne pour la sélection
  protected readonly selectedIds = signal<number[]>([]);

  //Gère le changement de sélection dans l'arborescence
  protected onSelectionChange(ids: number[]): void {
    this.selectedIds.set(ids);
    if (ids.length > 0) {
      const node = this.findNodeById(this.nodes(), ids[0]);
      if (node) {
        this.nodeSelected.emit({
          id: node.id,
          code: node.code,
          wording: node.wording,
          level: node.level,
          children: node.children,
          expanded: node.expanded,
        });
      }
    }
  }

  //Recherche récursive optimisée
  private findNodeById(nodes: ConstituencyNode[], id: number): ConstituencyNode | undefined {
    for (const node of nodes) {
      if (node.id === id) {
        return node;
      }
      if (node.children?.length) {
        const found = this.findNodeById(node.children, id);
        if (found) {
          return found;
        }
      }
    }
    return undefined;
  }
}
