/**
 * EXEMPLE D'UTILISATION - Comment utiliser ConstituencyTree dans le composant parent
 *
 * Ce fichier montre comment modifier constituency.ts pour utiliser le composant partagé
 */

import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { ConstituencyNode } from '@app/models/constituency.model';
import { ConstituencyApiService } from '@app/services/api/constituency.api.service';
import { GetConstituenciesResponse } from '@app/services/nswag/api-nswag-client';
import { ConstituencyTree } from '@app/shared/constituency-tree/constituency-tree';

@Component({
  selector: 'app-constituency',
  imports: [ConstituencyTree], // Import du composant partagé
  template: `
    <div class="constituency-page">
      <header class="page-header">
        <h1>Gestion des Circonscriptions</h1>

        <div class="toolbar">
          <button class="btn btn-secondary" (click)="expandAll()">📂 Tout étendre</button>
          <button class="btn btn-secondary" (click)="collapseAll()">📁 Tout réduire</button>
          <button class="btn btn-secondary" (click)="clearSelection()">
            ✖️ Effacer la sélection
          </button>
        </div>
      </header>

      <div class="content-layout">
        <!-- Arborescence -->
        <aside class="tree-panel">
          <app-constituency-tree
            [nodes]="nodes"
            [multiSelect]="false"
            [selectedIds]="selectedIds"
            (nodeSelected)="onNodeSelected($event)"
            (nodeExpanded)="onNodeExpanded($event)"
          >
          </app-constituency-tree>
        </aside>

        <!-- Panneau de détails -->
        <main class="details-panel">
          @if (selectedNode) {
            <div class="node-details">
              <h2>Détails de la circonscription</h2>

              <div class="detail-group">
                <label>ID:</label>
                <span>{{ selectedNode.id }}</span>
              </div>

              <div class="detail-group">
                <label>Code:</label>
                <span class="code">{{ selectedNode.code }}</span>
              </div>

              <div class="detail-group">
                <label>Libellé:</label>
                <span>{{ selectedNode.wording }}</span>
              </div>

              <div class="detail-group">
                <label>Niveau:</label>
                <span class="badge">{{ selectedNode.level }}</span>
              </div>

              @if (selectedNode.children && selectedNode.children.length > 0) {
                <div class="detail-group">
                  <label>Nombre d'enfants:</label>
                  <span>{{ selectedNode.children.length }}</span>
                </div>
              }

              <div class="actions">
                <button class="btn btn-primary">Modifier</button>
                <button class="btn btn-danger">Supprimer</button>
              </div>
            </div>
          } @else {
            <div class="empty-state">
              <p>Sélectionnez une circonscription pour voir ses détails</p>
            </div>
          }
        </main>
      </div>
    </div>
  `,
  styles: [
    `
      .constituency-page {
        height: 100%;
        display: flex;
        flex-direction: column;
      }

      .page-header {
        padding: 1.5rem;
        border-bottom: 1px solid #e0e0e0;
        background-color: #fafafa;

        h1 {
          margin: 0 0 1rem 0;
          font-size: 1.75rem;
        }
      }

      .toolbar {
        display: flex;
        gap: 0.5rem;

        .btn {
          padding: 0.5rem 1rem;
          border-radius: 4px;
          border: 1px solid #ddd;
          background: white;
          cursor: pointer;
          transition: all 0.2s;

          &:hover {
            background-color: #f5f5f5;
          }

          &.btn-secondary {
            font-size: 0.9rem;
          }
        }
      }

      .content-layout {
        flex: 1;
        display: grid;
        grid-template-columns: 350px 1fr;
        overflow: hidden;
      }

      .tree-panel {
        border-right: 1px solid #e0e0e0;
        overflow-y: auto;
        padding: 1rem;
        background-color: #fefefe;
      }

      .details-panel {
        overflow-y: auto;
        padding: 2rem;
      }

      .node-details {
        max-width: 600px;

        h2 {
          margin-top: 0;
          margin-bottom: 1.5rem;
          color: #333;
        }
      }

      .detail-group {
        display: flex;
        padding: 0.75rem 0;
        border-bottom: 1px solid #f0f0f0;

        label {
          font-weight: 600;
          width: 150px;
          color: #666;
        }

        span {
          flex: 1;
          color: #333;

          &.code {
            font-family: 'Courier New', monospace;
            background-color: #f5f5f5;
            padding: 0.25rem 0.5rem;
            border-radius: 3px;
          }

          &.badge {
            background-color: #1976d2;
            color: white;
            padding: 0.25rem 0.75rem;
            border-radius: 12px;
            font-size: 0.85rem;
            font-weight: 600;
            text-transform: uppercase;
          }
        }
      }

      .actions {
        margin-top: 2rem;
        display: flex;
        gap: 0.75rem;

        .btn {
          padding: 0.6rem 1.5rem;
          border: none;
          border-radius: 4px;
          cursor: pointer;
          font-weight: 500;
          transition: all 0.2s;

          &.btn-primary {
            background-color: #1976d2;
            color: white;

            &:hover {
              background-color: #1565c0;
            }
          }

          &.btn-danger {
            background-color: #dc3545;
            color: white;

            &:hover {
              background-color: #c82333;
            }
          }
        }
      }

      .empty-state {
        display: flex;
        align-items: center;
        justify-content: center;
        height: 100%;
        color: #999;
        font-size: 1.1rem;
      }

      @media (max-width: 768px) {
        .content-layout {
          grid-template-columns: 1fr;
          grid-template-rows: 300px 1fr;
        }

        .tree-panel {
          border-right: none;
          border-bottom: 1px solid #e0e0e0;
        }
      }
    `,
  ],
})
export class ConstituencyExample implements OnInit {
  @ViewChild(ConstituencyTree) tree!: ConstituencyTree;

  private readonly constituencyService = inject(ConstituencyApiService);

  nodes: ConstituencyNode[] = [];
  selectedNode: ConstituencyNode | null = null;
  selectedIds: number[] = [];

  ngOnInit(): void {
    this.loadConstituencies();
  }

  private loadConstituencies(): void {
    this.constituencyService.getConstituencyTree({}).subscribe({
      next: (response) => {
        const data = response.data;
        if (data && data.length > 0) {
          this.nodes = data.map((c) => this.mapToNode(c));

          // Optionnel: Pré-sélectionner le premier node
          // this.selectedIds = [this.nodes[0].id];
        }
      },
      error: (error) => {
        console.error('Erreur lors du chargement des circonscriptions:', error);
        // TODO: Afficher un message d'erreur à l'utilisateur
      },
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

  /**
   * Gère la sélection d'un node dans l'arborescence
   */
  onNodeSelected(node: ConstituencyNode | ConstituencyNode[] | null): void {
    // Dans ce cas, multiSelect est false, donc on reçoit un seul node
    this.selectedNode = node as ConstituencyNode | null;

    if (this.selectedNode) {
      console.log('Node sélectionné:', this.selectedNode);

      // Vous pouvez faire des actions supplémentaires ici:
      // - Charger des données supplémentaires
      // - Mettre à jour un formulaire
      // - Navigator vers une page de détails
      // - etc.
    }
  }

  /**
   * Gère l'expansion/réduction d'un node
   */
  onNodeExpanded(event: { node: ConstituencyNode; expanded: boolean }): void {
    console.log(`Node ${event.node.wording} ${event.expanded ? 'expansé' : 'réduit'}`);

    // Optionnel: Persister l'état d'expansion dans le localStorage
    // this.persistExpansionState(event.node.id, event.expanded);
  }

  /**
   * Expande tous les nodes de l'arborescence
   */
  expandAll(): void {
    if (this.tree) {
      this.tree.expandAll();
    }
  }

  /**
   * Réduit tous les nodes de l'arborescence
   */
  collapseAll(): void {
    if (this.tree) {
      this.tree.collapseAll();
    }
  }

  /**
   * Efface la sélection actuelle
   */
  clearSelection(): void {
    if (this.tree) {
      this.tree.clearSelection();
      this.selectedNode = null;
    }
  }
}

/**
 * EXEMPLE AVEC SÉLECTION MULTIPLE
 */
@Component({
  selector: 'app-constituency-multi-select-example',
  imports: [ConstituencyTree],
  template: `
    <app-constituency-tree
      [nodes]="nodes"
      [multiSelect]="true"
      (nodeSelected)="onMultipleNodesSelected($event)"
    >
    </app-constituency-tree>
  `,
})
export class ConstituencyMultiSelectExample {
  @ViewChild(ConstituencyTree) tree!: ConstituencyTree;

  nodes: ConstituencyNode[] = [];
  selectedNodes: ConstituencyNode[] = [];

  onMultipleNodesSelected(nodes: ConstituencyNode | ConstituencyNode[] | null): void {
    if (Array.isArray(nodes)) {
      this.selectedNodes = nodes;
      console.log(`${nodes.length} nodes sélectionnés:`, nodes);
    } else {
      this.selectedNodes = [];
    }
  }
}

/**
 * EXEMPLE AVEC CHARGEMENT LAZY (à la demande)
 */
@Component({
  selector: 'app-constituency-lazy-load-example',
  imports: [ConstituencyTree],
  template: `
    <app-constituency-tree [nodes]="nodes" (nodeExpanded)="onNodeExpanded($event)">
    </app-constituency-tree>
  `,
})
export class ConstituencyLazyLoadExample {
  nodes: ConstituencyNode[] = [];

  onNodeExpanded(event: { node: ConstituencyNode; expanded: boolean }): void {
    if (event.expanded && !event.node.children) {
      // Charger les enfants à la demande
      this.loadChildren(event.node);
    }
  }

  private loadChildren(node: ConstituencyNode): void {
    // Simuler un appel API
    // this.constituencyService.getChildren(node.id).subscribe(children => {
    //   node.children = children.map(c => this.mapToNode(c));
    // });
  }
}
