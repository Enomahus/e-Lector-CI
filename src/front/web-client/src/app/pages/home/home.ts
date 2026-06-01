import { CommonModule } from '@angular/common';
import {
  Component,
  computed,
  effect,
  ElementRef,
  inject,
  OnInit,
  signal,
  viewChild,
} from '@angular/core';
import { DashboardApiService } from '@app/services/api/dashboard.api.service';
import { GetDashboardStatsResponse } from '@app/services/nswag/api-nswag-client';
import { Loader } from '@app/shared/loader/loader';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import Chart from 'chart.js/auto';

export interface Tile {
  color: string;
  cols: number;
  rows: number;
  text: string;
}

@Component({
  selector: 'app-home',
  imports: [CommonModule, TranslateModule, Loader],
  templateUrl: './home.html',
  styleUrls: ['./home.scss'],
})
export class Home implements OnInit {
  private readonly dashboardService = inject(DashboardApiService);
  private readonly translateService = inject(TranslateService);

  // Éléments du DOM pour les graphiques (Syntaxe de requêtes de vue moderne Signal-based)
  private readonly regionChartCanvas = viewChild<ElementRef<HTMLCanvasElement>>('regionChart');
  private readonly ageChartCanvas = viewChild<ElementRef<HTMLCanvasElement>>('ageChart');

  statsState = signal<GetDashboardStatsResponse | null>(null);
  readonly isLoading = signal<boolean>(true);
  readonly hasError = signal<boolean>(false);

  // Signals Dérivés (Computed)
  readonly totalPopulation = computed(() => this.statsState()?.totalPopulation ?? 0);
  readonly genderStats = computed(() => this.statsState()?.genderStats);

  constructor() {
    effect(() => {
      const stats = this.statsState();
      const regionsCanvas = this.regionChartCanvas()?.nativeElement;
      const ageCanvas = this.ageChartCanvas()?.nativeElement;

      if (stats && regionsCanvas && ageCanvas) {
        // Initialiser les graphiques avec les données
        this.renderRegionChart(stats, regionsCanvas);
        this.renderAgeChart(stats, ageCanvas);
      }
    });
  }

  ngOnInit(): void {
    this.loadDashboardStats();
  }

  private loadDashboardStats(): void {
    this.dashboardService.getDashboardStats({}).subscribe({
      next: (resp) => {
        this.statsState.set(resp.data!);
        this.isLoading.set(false);
        this.hasError.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.hasError.set(true);
      },
    });
  }

  private renderRegionChart(stats: GetDashboardStatsResponse, canvas: HTMLCanvasElement): void {
    const labels = stats.regionGenderStats?.map((r) => r.region);
    const menData = stats.regionGenderStats?.map((r) => r.menCount);
    const womenData = stats.regionGenderStats?.map((r) => r.womenCount);

    const legendMen = this.translateService.instant('global.men');
    const legendWomen = this.translateService.instant('global.women');

    new Chart(canvas, {
      type: 'bar',
      data: {
        labels: labels,
        datasets: [
          { label: legendMen, data: menData, backgroundColor: '#1d5b79' },
          { label: legendWomen, data: womenData, backgroundColor: '#f26a36' },
        ],
      },
      options: {
        indexAxis: 'x', // Mode vertical comme sur l'image
        responsive: true,
        plugins: { legend: { position: 'bottom' } },
      },
    });
  }

  private renderAgeChart(stats: GetDashboardStatsResponse, canvas: HTMLCanvasElement): void {
    new Chart(canvas, {
      type: 'pie',
      data: {
        labels: stats.ageRangeStats?.map((a) => a.rangeLabel),
        datasets: [
          {
            data: stats.ageRangeStats?.map((a) => a.count),
            backgroundColor: ['#1d5b79', '#f26a36', '#1a6f2b', '#1093cd'],
          },
        ],
      },
      options: {
        responsive: true,
        plugins: { legend: { position: 'bottom' } },
      },
    });
  }
}
