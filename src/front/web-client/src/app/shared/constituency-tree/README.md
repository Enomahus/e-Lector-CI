# Constituency Tree Component

Composant Angular réutilisable pour afficher une arborescence hiérarchique de circonscriptions.

## Fonctionnalités

✅ **Sélection simple ou multiple** - Mode configurable via `[multiSelect]`  
✅ **Expansion/Collapse** - Gestion automatique de l'état d'expansion  
✅ **API publique** - Méthodes pour contrôler le composant programmatiquement  
✅ **TypeScript strict** - Typage complet avec interfaces  
✅ **Signals Angular** - Gestion réactive de l'état  
✅ **Standalone component** - Prêt pour l'import direct  
✅ **Accessible** - Attributs ARIA et navigation clavier  
✅ **Responsive** - S'adapte aux petits écrans  
✅ **Dark mode ready** - Support du mode sombre  

## Utilisation

### Import du composant

```typescript
import { ConstituencyTree } from '@app/shared/constituency-tree/constituency-tree';

@Component({
  imports: [ConstituencyTree, /* autres imports */],
  // ...
})
```

### Exemple basique

```html
<app-constituency-tree
  [nodes]="constituencyNodes"
  (nodeSelected)="onNodeSelected($event)">
</app-constituency-tree>
```

```typescript
export class MyComponent {
  constituencyNodes: ConstituencyNode[] = [];

  onNodeSelected(node: ConstituencyNode | null): void {
    if (node) {
      console.log('Node sélectionné:', node);
      console.log('ID:', node.id);
      console.log('Code:', node.code);
      console.log('Libellé:', node.wording);
      console.log('Niveau:', node.level);
    }
  }
}
```

### Sélection multiple

```html
<app-constituency-tree
  [nodes]="constituencyNodes"
  [multiSelect]="true"
  (nodeSelected)="onMultipleNodesSelected($event)">
</app-constituency-tree>
```

```typescript
onMultipleNodesSelected(nodes: ConstituencyNode[] | null): void {
  if (nodes) {
    console.log(`${nodes.length} nodes sélectionnés:`, nodes);
  }
}
```

### Sélection initiale

```html
<app-constituency-tree
  [nodes]="constituencyNodes"
  [selectedIds]="[1, 5, 10]"
  (nodeSelected)="onNodeSelected($event)">
</app-constituency-tree>
```

### Écoute des expansions

```html
<app-constituency-tree
  [nodes]="constituencyNodes"
  (nodeExpanded)="onNodeExpanded($event)"
  (nodeSelected)="onNodeSelected($event)">
</app-constituency-tree>
```

```typescript
onNodeExpanded(event: { node: ConstituencyNode; expanded: boolean }): void {
  console.log(
    `Node ${event.node.wording} ${event.expanded ? 'expansé' : 'réduit'}`
  );
}
```

### Style personnalisé

```html
<app-constituency-tree
  [nodes]="constituencyNodes"
  [cssClass]="'custom-tree-style'"
  (nodeSelected)="onNodeSelected($event)">
</app-constituency-tree>
```

```scss
.custom-tree-style {
  max-height: 500px;
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 1rem;
}
```

## API Publique

### Inputs

| Propriété | Type | Requis | Défaut | Description |
|-----------|------|--------|--------|-------------|
| `nodes` | `ConstituencyNode[]` | ✅ Oui | `[]` | Données de l'arborescence |
| `multiSelect` | `boolean` | Non | `false` | Active la sélection multiple |
| `selectedIds` | `number[]` | Non | `[]` | IDs des nodes pré-sélectionnés |
| `cssClass` | `string` | Non | `''` | Classe CSS personnalisée |

### Outputs

| Event | Type | Description |
|-------|------|-------------|
| `nodeSelected` | `EventEmitter<ConstituencyNode \| ConstituencyNode[] \| null>` | Émis lors d'une sélection/désélection |
| `nodeExpanded` | `EventEmitter<{ node: ConstituencyNode; expanded: boolean }>` | Émis lors de l'expansion/réduction |

### Méthodes publiques

Accessible via `@ViewChild` :

```typescript
@ViewChild(ConstituencyTree) tree!: ConstituencyTree;

// Expande tous les nodes
this.tree.expandAll();

// Réduit tous les nodes
this.tree.collapseAll();

// Sélectionne un node par ID
this.tree.selectNode(5);

// Efface toutes les sélections
this.tree.clearSelection();
```

## Exemple complet

```typescript
import { Component, OnInit, ViewChild, inject } from '@angular/core';
import { ConstituencyTree } from '@app/shared/constituency-tree/constituency-tree';
import { ConstituencyNode } from '@app/models/constituency.model';
import { ConstituencyApiService } from '@app/services/api/constituency.api.service';

@Component({
  selector: 'app-my-page',
  imports: [ConstituencyTree],
  template: `
    <div class="page-container">
      <h1>Sélection de circonscription</h1>
      
      <div class="controls">
        <button (click)="tree.expandAll()">Tout étendre</button>
        <button (click)="tree.collapseAll()">Tout réduire</button>
        <button (click)="tree.clearSelection()">Effacer sélection</button>
      </div>

      <app-constituency-tree
        [nodes]="nodes"
        [multiSelect]="allowMultiple"
        [selectedIds]="preselectedIds"
        (nodeSelected)="handleSelection($event)"
        (nodeExpanded)="handleExpansion($event)">
      </app-constituency-tree>

      @if (selectedNode) {
        <div class="selection-info">
          <h3>Node sélectionné:</h3>
          <p><strong>Code:</strong> {{ selectedNode.code }}</p>
          <p><strong>Libellé:</strong> {{ selectedNode.wording }}</p>
          <p><strong>Niveau:</strong> {{ selectedNode.level }}</p>
        </div>
      }
    </div>
  `,
})
export class MyPageComponent implements OnInit {
  @ViewChild(ConstituencyTree) tree!: ConstituencyTree;
  
  private readonly constituencyService = inject(ConstituencyApiService);

  nodes: ConstituencyNode[] = [];
  selectedNode: ConstituencyNode | null = null;
  allowMultiple = false;
  preselectedIds = [1, 3];

  ngOnInit(): void {
    this.loadConstituencies();
  }

  private loadConstituencies(): void {
    this.constituencyService.getConstituencyTree({}).subscribe((response) => {
      this.nodes = response.data?.map(c => this.mapToNode(c)) || [];
    });
  }

  handleSelection(node: ConstituencyNode | ConstituencyNode[] | null): void {
    if (Array.isArray(node)) {
      console.log('Plusieurs nodes sélectionnés:', node);
      this.selectedNode = node[0] || null;
    } else {
      this.selectedNode = node;
    }
  }

  handleExpansion(event: { node: ConstituencyNode; expanded: boolean }): void {
    console.log(`${event.node.wording}: ${event.expanded ? 'expansé' : 'réduit'}`);
  }

  private mapToNode(constituency: any): ConstituencyNode {
    return {
      id: constituency.id!,
      code: constituency.code!,
      wording: constituency.wording!,
      level: constituency.level!,
      children: constituency.children?.map(c => this.mapToNode(c)),
      expanded: false,
    };
  }
}
```

## Personnalisation du style

### Variables CSS

Le composant utilise des variables CSS personnalisables :

```css
:root {
  --color-primary: #1976d2;
  --color-primary-light: #e3f2fd;
  --color-primary-dark: #1565c0;
  --color-hover: #f5f5f5;
  --color-active: #eeeeee;
  --color-text-primary: #333;
  --color-text-secondary: #666;
  --color-badge-bg: #e0e0e0;
  --color-badge-text: #555;
}
```

### Override des styles

```scss
app-constituency-tree {
  ::ng-deep {
    .tree-item-content {
      padding: 0.6rem;
      border-left: 2px solid transparent;
      
      &:hover {
        border-left-color: var(--color-primary);
      }
    }

    .tree-item-badge {
      display: none; // Cache les badges de niveau
    }
  }
}
```

## Tests

### Test unitaire

```typescript
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ConstituencyTree } from './constituency-tree';
import { ConstituencyNode } from '@app/models/constituency.model';

describe('ConstituencyTree', () => {
  let component: ConstituencyTree;
  let fixture: ComponentFixture<ConstituencyTree>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConstituencyTree],
    }).compileComponents();

    fixture = TestBed.createComponent(ConstituencyTree);
    component = fixture.componentInstance;
  });

  it('should emit nodeSelected on selection', (done) => {
    const mockNodes: ConstituencyNode[] = [
      { id: 1, code: 'A1', wording: 'Node 1', level: 'City' },
    ];
    component.nodes = mockNodes;
    
    component.nodeSelected.subscribe((node) => {
      expect(node).toBeTruthy();
      expect((node as ConstituencyNode).id).toBe(1);
      done();
    });

    component.selectNode(1);
  });
});
```

## Architecture

```
constituency-tree/
├── constituency-tree.ts        # Composant principal (logique)
├── constituency-tree.html      # Template Angular
├── constituency-tree.scss      # Styles
└── README.md                   # Documentation
```

## Bonnes pratiques

1. **Toujours fournir un `track by`** - Déjà géré avec `track node.id`
2. **Ne pas muter les inputs** - Le composant clone les données
3. **Utiliser `@ViewChild` pour l'API programmatique**
4. **Gérer les états vides** - Vérifier si `nodes` existe
5. **Performance** - Le composant utilise OnPush change detection

## Support navigateur

- ✅ Chrome (dernière version)
- ✅ Firefox (dernière version)
- ✅ Safari (dernière version)
- ✅ Edge (dernière version)

## Licence

Conforme à la licence du projet e-Lector-CI.
