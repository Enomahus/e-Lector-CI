import { Tree, TreeItem, TreeItemGroup } from '@angular/aria/tree';
import { NgTemplateOutlet } from '@angular/common';
import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  signal,
  SimpleChanges,
} from '@angular/core';
import { ConstituencyNode } from '@app/models/constituency.model';

/**
 * Reusable constituency tree component that displays hierarchical location data.
 *
 * @example
 * ```html
 * <app-constituency-tree
 *   [nodes]="constituencyNodes"
 *   [multiSelect]="false"
 *   (nodeSelected)="onNodeSelected($event)">
 * </app-constituency-tree>
 * ```
 */
@Component({
  selector: 'app-constituency-tree',
  imports: [Tree, TreeItem, TreeItemGroup, NgTemplateOutlet],
  templateUrl: './constituency-tree.html',
  styleUrl: './constituency-tree.scss',
})
export class ConstituencyTree implements OnChanges {
  /**
   * Array of constituency nodes to display in the tree
   */
  @Input({ required: true }) nodes: ConstituencyNode[] = [];

  /**
   * Enable multi-selection mode
   * @default false
   */
  @Input() multiSelect = false;

  /**
   * Initially selected node IDs
   */
  @Input() selectedIds: number[] = [];

  /**
   * CSS class to apply to the tree container
   */
  @Input() cssClass = '';

  /**
   * Emits when a node is selected or deselected
   * Returns the selected node(s) or null if deselected
   */
  @Output() nodeSelected = new EventEmitter<ConstituencyNode | ConstituencyNode[] | null>();

  /**
   * Emits when node expansion state changes
   */
  @Output() nodeExpanded = new EventEmitter<{ node: ConstituencyNode; expanded: boolean }>();

  protected readonly internalNodes = signal<ConstituencyNode[]>([]);
  protected readonly selected = signal<string[]>([]);

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['nodes'] && this.nodes) {
      // Deep clone nodes to avoid mutating input
      this.internalNodes.set(this.deepCloneNodes(this.nodes));
    }

    if (changes['selectedIds'] && this.selectedIds) {
      this.selected.set(this.selectedIds.map((id) => id.toString()));
    }
  }

  /**
   * Handles node selection changes
   */
  protected onSelectionChange(selectedIds: string[]): void {
    this.selected.set(selectedIds);

    if (selectedIds.length === 0) {
      this.nodeSelected.emit(null);
      return;
    }

    const selectedNodes = this.findNodesByIds(
      selectedIds.map((id) => parseInt(id, 10)),
      this.internalNodes(),
    );

    if (this.multiSelect) {
      this.nodeSelected.emit(selectedNodes);
    } else {
      this.nodeSelected.emit(selectedNodes[0] || null);
    }
  }

  /**
   * Handles node expansion/collapse
   */
  protected onNodeToggle(node: ConstituencyNode, expanded: boolean): void {
    node.expanded = expanded;
    this.nodeExpanded.emit({ node, expanded });
  }

  /**
   * Deep clones nodes array to prevent mutation of input data
   */
  private deepCloneNodes(nodes: ConstituencyNode[]): ConstituencyNode[] {
    return nodes.map((node) => ({
      ...node,
      children: node.children ? this.deepCloneNodes(node.children) : undefined,
    }));
  }

  /**
   * Recursively finds nodes by their IDs
   */
  private findNodesByIds(ids: number[], nodes: ConstituencyNode[]): ConstituencyNode[] {
    const found: ConstituencyNode[] = [];

    for (const node of nodes) {
      if (ids.includes(node.id)) {
        found.push(node);
      }

      if (node.children && node.children.length > 0) {
        found.push(...this.findNodesByIds(ids, node.children));
      }
    }

    return found;
  }

  /**
   * Expands all nodes in the tree
   */
  public expandAll(): void {
    this.setAllNodesExpanded(this.internalNodes(), true);
    this.internalNodes.set([...this.internalNodes()]);
  }

  /**
   * Collapses all nodes in the tree
   */
  public collapseAll(): void {
    this.setAllNodesExpanded(this.internalNodes(), false);
    this.internalNodes.set([...this.internalNodes()]);
  }

  /**
   * Recursively sets expansion state for all nodes
   */
  private setAllNodesExpanded(nodes: ConstituencyNode[], expanded: boolean): void {
    nodes.forEach((node) => {
      if (node.children && node.children.length > 0) {
        node.expanded = expanded;
        this.setAllNodesExpanded(node.children, expanded);
      }
    });
  }

  /**
   * Programmatically selects a node by ID
   */
  public selectNode(id: number): void {
    if (this.multiSelect) {
      const currentIds = this.selected();
      const stringId = id.toString();
      if (!currentIds.includes(stringId)) {
        this.selected.set([...currentIds, stringId]);
        this.onSelectionChange(this.selected());
      }
    } else {
      this.selected.set([id.toString()]);
      this.onSelectionChange(this.selected());
    }
  }

  /**
   * Clears all selections
   */
  public clearSelection(): void {
    this.selected.set([]);
    this.nodeSelected.emit(null);
  }
}
