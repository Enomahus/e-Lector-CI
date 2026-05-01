import { LocationLevel } from '@app/services/nswag/api-nswag-client';

export interface ConstituencyNode {
  id: number;
  code: string;
  wording: string;
  parentId?: number;
  level: LocationLevel;
  children?: ConstituencyNode[];
  expanded?: boolean;
}
