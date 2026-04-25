/**
 * Constituency Tree Component
 *
 * Composant Angular réutilisable pour afficher une arborescence hiérarchique de circonscriptions.
 *
 * @example
 * ```typescript
 * import { ConstituencyTree } from '@app/shared/constituency-tree';
 *
 * @Component({
 *   imports: [ConstituencyTree],
 *   template: `
 *     <app-constituency-tree
 *       [nodes]="nodes"
 *       (nodeSelected)="onNodeSelected($event)">
 *     </app-constituency-tree>
 *   `
 * })
 * ```
 */

export { ConstituencyTree } from './constituency-tree';
