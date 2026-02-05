import { Component, computed, inject, signal } from '@angular/core';
import { LocationLevel } from '../../../enums/location-level.enum';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-geographic-area',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './geographic-area.html',
  styleUrl: './geographic-area.scss',
})
export class GeographicArea {
  private fb = inject(FormBuilder);
  
  // Simulation d'une base de données existante (Signals)
  allAreas = signal([
    { id: 1, name: 'DIASPORA', level: LocationLevel.Department, levelName: 'DEPT' },
    { id: 2, name: 'EUROPE', level: LocationLevel.SubPrefecture, levelName: 'SP', parentId: 1 },
    { id: 3, name: 'ABIDJAN', level: LocationLevel.Department, levelName: 'DEPT' }
  ]);

  levels = [
    { label: 'Continent', value: LocationLevel.Continent },
    { label: 'Département', value: LocationLevel.Department },
    { label: 'Sous-Préfecture', value: LocationLevel.SubPrefecture },
    { label: 'Pays', value: LocationLevel.Country },
    { label: 'Commune / Ville', value: LocationLevel.City },
  ];

  areaForm = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    level: [null as LocationLevel | null, Validators.required],
    parentId: [null as number | null]
  });

  // Signal pour suivre le niveau sélectionné
  selectedLevel = signal<LocationLevel | null>(null);

  // Calculé : Est-ce que le niveau nécessite un parent ?
  isNotRoot = computed(() => {
    const level = this.selectedLevel();
    return level !== null && level !== LocationLevel.Continent;
  });

  // Calculé : Liste des parents éligibles (Niveau inférieur au niveau choisi)
  availableParents = computed(() => {
    const currentLevel = this.selectedLevel();
    if (!currentLevel) return [];
    // Un parent doit avoir un niveau numériquement inférieur (ex: un niveau 3 a un parent de niveau 2 ou 1)
    return this.allAreas().filter(area => area.level < currentLevel);
  });

  constructor() {
    // Liaison entre le formulaire et les signaux
    this.areaForm.get('level')?.valueChanges.subscribe(val => {
      const numericLevel = val ? Number(val) : null;
      this.selectedLevel.set(numericLevel);
      
      // Validation dynamique du ParentId
      if (numericLevel && numericLevel !== LocationLevel.Continent) {
        this.areaForm.get('parentId')?.setValidators(Validators.required);
      } else {
        this.areaForm.get('parentId')?.clearValidators();
        this.areaForm.patchValue({ parentId: null });
      }
      this.areaForm.get('parentId')?.updateValueAndValidity();
    });
  }

  saveArea() {
    if (this.areaForm.valid) {
      console.log('Envoi à l\'API .NET 10:', this.areaForm.value);
      // Logique de sauvegarde ici
    }
  }
}
