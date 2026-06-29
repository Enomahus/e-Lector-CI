import { Tree, TreeItem, TreeItemGroup } from '@angular/aria/tree';
import { CdkMenu, CdkMenuItem, CdkMenuTrigger } from '@angular/cdk/menu';
import { NgTemplateOutlet } from '@angular/common';
import { Component, effect, input, output, signal, untracked } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { ConstituencyNode } from '../../models/constituency.model';
import { PermissionDirective } from '../../services/auth/permission.directive';

@Component({
  selector: 'app-constituency-tree',
  imports: [
    Tree,
    TreeItem,
    TreeItemGroup,
    NgTemplateOutlet,
    TranslateModule,
    CdkMenuTrigger,
    CdkMenu,
    CdkMenuItem,
    PermissionDirective,
  ],
  templateUrl: './constituency-tree.html',
  styleUrl: './constituency-tree.scss',
})
export class ConstituencyTree {
  nodes = input.required<ConstituencyNode[]>();
  initialSelectedId = input<number | undefined>(undefined);
  // Outputs pour les actions du menu
  nodeSelected = output<ConstituencyNode>();
  editNode = output<ConstituencyNode>();
  deleteNode = output<ConstituencyNode>();
  toggleStatus = output<ConstituencyNode>();

  protected readonly selectedIds = signal<number[]>([]);

  constructor() {
    effect(() => {
      const id = this.initialSelectedId();
      if (id !== undefined) {
        untracked(() => this.selectedIds.set([id]));
      }
    });
  }

  //Gère le changement de sélection dans l'arborescence
  protected onSelectionChange(ids: number[]): void {
    if (ids.length === 0) return;

    const nodeId = ids[0];
    let node = this.findNodeById(this.nodes(), nodeId);

    if (node && this.isLeaf(node)) {
      this.selectedIds.set(ids);
      this.nodeSelected.emit(node);
    } else {
      node = undefined;
    }
  }

  // Actions du menu
  onEdit(node: ConstituencyNode): void {
    this.editNode.emit(node);
  }
  onDelete(node: ConstituencyNode): void {
    this.deleteNode.emit(node);
  }
  onToggleStatus(node: ConstituencyNode): void {
    this.toggleStatus.emit(node);
  }

  isLeaf(node: ConstituencyNode): boolean {
    return (
      node.children === undefined || node.children.length === 0 //&& node.level === 'votingLocation'
    );
  }

  //Recherche récursive optimisée
  private findNodeById(nodes: ConstituencyNode[], id: number): ConstituencyNode | undefined {
    for (const node of nodes) {
      if (node.id === id) return node;

      if (node.children?.length) {
        const found = this.findNodeById(node.children, id);
        if (found) return found;
      }
    }
    return undefined;
  }
}
