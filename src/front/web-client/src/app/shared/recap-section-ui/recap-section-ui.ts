import { CommonModule } from '@angular/common';
import { Component, computed, input, output } from '@angular/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';

export interface RecapItem {
  label: string;
  value: string | number | boolean | null | undefined;
}

@Component({
  selector: 'app-recap-section-ui',
  imports: [CommonModule, MatExpansionModule, MatIconModule],
  templateUrl: './recap-section-ui.html',
  styleUrl: './recap-section-ui.scss',
})
export class RecapSectionUi {
  // Signal Inputs modernes (Angular 17.2+)
  title = input.required<string>();
  step = input.required<number>();
  color = input.required<string>();
  items = input.required<RecapItem[]>();

  // Émetteur d'événement moderne (Angular 17.3+)
  edit = output<number>();

  // Filtrage réactif performant des données (Remplace le .map/.filter inline de React)
  protected filteredItems = computed(() =>
    this.items().filter(
      (item) => item.value !== null && item.value !== undefined && item.value !== '',
    ),
  );

  protected isBoolean(value: unknown): value is boolean {
    return typeof value === 'boolean';
  }
}
