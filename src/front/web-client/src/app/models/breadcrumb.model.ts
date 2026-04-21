import { Params } from '@angular/router';

export interface Breadcrumbs {
  label: string;
  url?: string;
  queryParams?: Params;
}
