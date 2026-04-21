import { Tree, TreeItem, TreeItemGroup } from '@angular/aria/tree';
import { NgTemplateOutlet } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { ConstituencyNode } from '@app/models/constituency.model';
import { ConstituencyApiService } from '@app/services/api/constituency.api.service';
import { GetConstituenciesResponse } from '@app/services/nswag/api-nswag-client';

@Component({
  selector: 'app-constituency',
  imports: [Tree, TreeItem, TreeItemGroup, NgTemplateOutlet],
  templateUrl: './constituency.html',
  styleUrl: './constituency.scss',
})
export class Constituency implements OnInit {
  private readonly constituecyService = inject(ConstituencyApiService);

  readonly nodes = signal<ConstituencyNode[]>([]);
  readonly selected = signal<string[]>([]);

  ngOnInit(): void {
    this.constituecyService.getConstituencyTree({}).subscribe((response) => {
      const res = response.data;
      this.nodes.set(res!.map((c) => this.mapToNode(c)));
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
}
