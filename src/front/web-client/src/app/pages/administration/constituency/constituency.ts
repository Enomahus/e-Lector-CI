import { CdkTreeModule, NestedTreeControl } from '@angular/cdk/tree';
import { Component, inject, signal } from '@angular/core';
import { ConstituencyApiService } from '@app/services/api/constituency.api.service';
import { GetConstituenciesResponse } from '@app/services/nswag/api-nswag-client';

@Component({
  selector: 'app-constituency',
  imports: [CdkTreeModule],
  templateUrl: './constituency.html',
  styleUrl: './constituency.scss',
})
export class Constituency {
  private readonly constituecyService = inject(ConstituencyApiService);

  treeControl = new NestedTreeControl<GetConstituenciesResponse>((node) => node.subConstituencies);
  dataSource = signal<GetConstituenciesResponse[]>([]);

  constructor() {
    this.constituecyService.getConstituencies({}).subscribe((data) => {
      this.dataSource.set(data);
    });
  }
  hasChild = (_: number, node: GetConstituenciesResponse) =>
    !!node.subConstituencies && node.subConstituencies.length > 0;
}
