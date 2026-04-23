import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-polling-stations',
  imports: [TranslateModule, RouterLink],
  templateUrl: './polling-stations.html',
  styleUrl: './polling-stations.scss',
})
export class PollingStations {
  private readonly title = 'Polling Stations';
}
