import { Component, inject, signal } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { RouterLink } from '@angular/router';
import { PollingStationApiService } from '@app/services/api/polling-station.api.service';
import {
  GetPollingStationsQuery,
  GetPollingStationsResponse,
} from '@app/services/nswag/api-nswag-client';
import { BaseTable } from '@app/shared/base-table/base-table';
import { TranslateModule } from '@ngx-translate/core';
import { map, Observable } from 'rxjs';

@Component({
  selector: 'app-polling-stations',
  imports: [
    TranslateModule,
    RouterLink,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './polling-stations.html',
  styleUrl: './polling-stations.scss',
})
export class PollingStations extends BaseTable<GetPollingStationsResponse> {
  private readonly pollingStationService = inject(PollingStationApiService);

  displayedColumns = signal<string[]>([
    'stationNumber',
    'votingLocationName',
    'municipalityName',
    'subPrefectureName',
    'departmentName',
    'regionName',
  ]);
  // dataSource = signal<GetPollingStationsResponse[]>([]);
  filteredDataSource = signal(new MatTableDataSource<GetPollingStationsResponse>());

  //clickedRows = new Set<GetPollingStationsResponse>();
  // @ViewChild(MatPaginator) paginator!: MatPaginator;
  // @ViewChild(MatSort) sort!: MatSort;

  constructor() {
    super();
  }

  override getData(
    sort: string,
    order: string,
    page: number,
  ): Observable<{ data: GetPollingStationsResponse[]; total: number }> {
    const query: GetPollingStationsQuery = {
      sort,
      order,
      pageIndex: page,
    };
    return this.pollingStationService.getPollingStations(query).pipe(
      map((response) => ({
        data: response.data?.items ?? [],
        total: response.data?.totalCount ?? 0,
      })),
    );
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value.trim().toLowerCase();
    this.filteredDataSource().filter = filterValue;
  }
}
