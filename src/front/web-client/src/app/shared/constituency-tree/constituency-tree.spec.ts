import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ConstituencyNode } from '@app/models/constituency.model';
import { ConstituencyTree } from './constituency-tree';

describe('ConstituencyTree', () => {
  let component: ConstituencyTree;
  let fixture: ComponentFixture<ConstituencyTree>;

  const mockNodes: ConstituencyNode[] = [
    {
      id: 1,
      code: 'A1',
      wording: 'Node 1',
      level: LocationLevel.City,
      expanded: false,
      children: [
        {
          id: 2,
          code: 'A1-1',
          wording: 'Node 1.1',
          level: LocationLevel.District,
          expanded: false,
        },
        {
          id: 3,
          code: 'A1-2',
          wording: 'Node 1.2',
          level: LocationLevel.District,
          expanded: false,
        },
      ],
    },
    {
      id: 4,
      code: 'B1',
      wording: 'Node 2',
      level: LocationLevel.City,
      expanded: false,
    },
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConstituencyTree],
    }).compileComponents();

    fixture = TestBed.createComponent(ConstituencyTree);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Initialization', () => {
    it('should initialize with empty nodes', () => {
      expect(component.nodes).toEqual([]);
      expect(component.internalNodes()).toEqual([]);
    });

    it('should initialize with default values', () => {
      expect(component.multiSelect).toBe(false);
      expect(component.selectedIds).toEqual([]);
      expect(component.cssClass).toBe('');
    });

    it('should clone nodes on ngOnChanges', () => {
      component.nodes = mockNodes;
      component.ngOnChanges({
        nodes: {
          currentValue: mockNodes,
          previousValue: null,
          firstChange: true,
          isFirstChange: () => true,
        },
      });

      expect(component.internalNodes()).toEqual(mockNodes);
      expect(component.internalNodes()).not.toBe(mockNodes); // Vérifie que c'est un clone
    });

    it('should set selected IDs on ngOnChanges', () => {
      component.selectedIds = [1, 3];
      component.ngOnChanges({
        selectedIds: {
          currentValue: [1, 3],
          previousValue: [],
          firstChange: false,
          isFirstChange: () => false,
        },
      });

      expect(component.selected()).toEqual(['1', '3']);
    });
  });

  describe('Node Selection', () => {
    beforeEach(() => {
      component.nodes = mockNodes;
      component.ngOnChanges({
        nodes: {
          currentValue: mockNodes,
          previousValue: null,
          firstChange: true,
          isFirstChange: () => true,
        },
      });
    });

    it('should emit null when selection is cleared', (done) => {
      component.nodeSelected.subscribe((node) => {
        expect(node).toBeNull();
        done();
      });

      component.onSelectionChange([]);
    });

    it('should emit single node in single-select mode', (done) => {
      component.multiSelect = false;

      component.nodeSelected.subscribe((node) => {
        expect(node).toBeTruthy();
        expect((node as ConstituencyNode).id).toBe(1);
        expect((node as ConstituencyNode).wording).toBe('Node 1');
        done();
      });

      component.onSelectionChange(['1']);
    });

    it('should emit array of nodes in multi-select mode', (done) => {
      component.multiSelect = true;

      component.nodeSelected.subscribe((nodes) => {
        expect(Array.isArray(nodes)).toBe(true);
        expect((nodes as ConstituencyNode[]).length).toBe(2);
        expect((nodes as ConstituencyNode[])[0].id).toBe(1);
        expect((nodes as ConstituencyNode[])[1].id).toBe(2);
        done();
      });

      component.onSelectionChange(['1', '2']);
    });

    it('should find nested nodes correctly', (done) => {
      component.multiSelect = false;

      component.nodeSelected.subscribe((node) => {
        expect(node).toBeTruthy();
        expect((node as ConstituencyNode).id).toBe(2);
        expect((node as ConstituencyNode).wording).toBe('Node 1.1');
        done();
      });

      component.onSelectionChange(['2']);
    });
  });

  describe('Node Expansion', () => {
    beforeEach(() => {
      component.nodes = mockNodes;
      component.ngOnChanges({
        nodes: {
          currentValue: mockNodes,
          previousValue: null,
          firstChange: true,
          isFirstChange: () => true,
        },
      });
    });

    it('should emit nodeExpanded event when node is expanded', (done) => {
      const node = component.internalNodes()[0];

      component.nodeExpanded.subscribe((event) => {
        expect(event.node).toBe(node);
        expect(event.expanded).toBe(true);
        done();
      });

      component.onNodeToggle(node, true);
    });

    it('should update node expanded state', () => {
      const node = component.internalNodes()[0];
      expect(node.expanded).toBe(false);

      component.onNodeToggle(node, true);
      expect(node.expanded).toBe(true);

      component.onNodeToggle(node, false);
      expect(node.expanded).toBe(false);
    });
  });

  describe('Public API Methods', () => {
    beforeEach(() => {
      component.nodes = mockNodes;
      component.ngOnChanges({
        nodes: {
          currentValue: mockNodes,
          previousValue: null,
          firstChange: true,
          isFirstChange: () => true,
        },
      });
    });

    describe('expandAll()', () => {
      it('should expand all nodes with children', () => {
        component.expandAll();

        const rootNode = component.internalNodes()[0];
        expect(rootNode.expanded).toBe(true);
      });

      it('should not affect leaf nodes', () => {
        component.expandAll();

        const leafNode = component.internalNodes()[1];
        expect(leafNode.expanded).toBe(false);
      });
    });

    describe('collapseAll()', () => {
      it('should collapse all expanded nodes', () => {
        // Expand first
        component.expandAll();
        expect(component.internalNodes()[0].expanded).toBe(true);

        // Then collapse
        component.collapseAll();
        expect(component.internalNodes()[0].expanded).toBe(false);
      });
    });

    describe('selectNode()', () => {
      it('should select a node by ID in single-select mode', (done) => {
        component.multiSelect = false;

        component.nodeSelected.subscribe((node) => {
          expect((node as ConstituencyNode).id).toBe(1);
          done();
        });

        component.selectNode(1);
      });

      it('should add to selection in multi-select mode', (done) => {
        component.multiSelect = true;
        component.selected.set(['1']);

        component.nodeSelected.subscribe((nodes) => {
          expect((nodes as ConstituencyNode[]).length).toBe(2);
          expect((nodes as ConstituencyNode[]).some((n) => n.id === 4)).toBe(true);
          done();
        });

        component.selectNode(4);
      });

      it('should not duplicate selections in multi-select mode', () => {
        component.multiSelect = true;
        component.selected.set(['1']);

        component.selectNode(1);

        expect(component.selected().length).toBe(1);
      });
    });

    describe('clearSelection()', () => {
      it('should clear all selections and emit null', (done) => {
        component.selected.set(['1', '2']);

        component.nodeSelected.subscribe((node) => {
          expect(node).toBeNull();
          done();
        });

        component.clearSelection();
      });

      it('should reset selected signal', () => {
        component.selected.set(['1', '2']);
        component.clearSelection();

        expect(component.selected()).toEqual([]);
      });
    });
  });

  describe('Deep Cloning', () => {
    it('should not mutate input nodes', () => {
      const originalNodes = JSON.parse(JSON.stringify(mockNodes));
      component.nodes = mockNodes;
      component.ngOnChanges({
        nodes: {
          currentValue: mockNodes,
          previousValue: null,
          firstChange: true,
          isFirstChange: () => true,
        },
      });

      // Modify internal nodes
      component.internalNodes()[0].expanded = true;
      component.internalNodes()[0].wording = 'Modified';

      // Original should remain unchanged
      expect(mockNodes[0].expanded).toBe(originalNodes[0].expanded);
      expect(mockNodes[0].wording).toBe(originalNodes[0].wording);
    });

    it('should deep clone nested children', () => {
      component.nodes = mockNodes;
      component.ngOnChanges({
        nodes: {
          currentValue: mockNodes,
          previousValue: null,
          firstChange: true,
          isFirstChange: () => true,
        },
      });

      // Modify nested child
      const internalChild = component.internalNodes()[0].children![0];
      internalChild.wording = 'Modified Child';

      // Original child should remain unchanged
      expect(mockNodes[0].children![0].wording).toBe('Node 1.1');
    });
  });

  describe('Edge Cases', () => {
    it('should handle empty nodes array', () => {
      component.nodes = [];
      component.ngOnChanges({
        nodes: {
          currentValue: [],
          previousValue: null,
          firstChange: true,
          isFirstChange: () => true,
        },
      });

      expect(component.internalNodes()).toEqual([]);
    });

    it('should handle selection of non-existent node', (done) => {
      component.nodes = mockNodes;
      component.ngOnChanges({
        nodes: {
          currentValue: mockNodes,
          previousValue: null,
          firstChange: true,
          isFirstChange: () => true,
        },
      });

      component.nodeSelected.subscribe((node) => {
        expect(node).toBeNull();
        done();
      });

      component.onSelectionChange(['999']);
    });

    it('should handle nodes without children', () => {
      const noChildrenNodes: ConstituencyNode[] = [
        {
          id: 1,
          code: 'A1',
          wording: 'Leaf Node',
          level: LocationLevel.City,
        },
      ];

      component.nodes = noChildrenNodes;
      component.ngOnChanges({
        nodes: {
          currentValue: noChildrenNodes,
          previousValue: null,
          firstChange: true,
          isFirstChange: () => true,
        },
      });

      expect(component.internalNodes()[0].children).toBeUndefined();
    });
  });

  describe('Template Integration', () => {
    it('should render tree structure', () => {
      component.nodes = mockNodes;
      fixture.detectChanges();

      const compiled = fixture.nativeElement as HTMLElement;
      const treeElement = compiled.querySelector('[ngTree]');

      expect(treeElement).toBeTruthy();
    });

    it('should apply custom CSS class', () => {
      component.cssClass = 'custom-tree-class';
      fixture.detectChanges();

      const container = fixture.nativeElement.querySelector('.constituency-tree-container');
      expect(container?.classList.contains('custom-tree-class')).toBe(true);
    });
  });
});
