import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { form, FormField, min, minLength, required } from '@angular/forms/signals';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-polling-station',
  imports: [CommonModule, FormField, ReactiveFormsModule],
  templateUrl: './polling-station.html',
  styleUrl: './polling-station.scss',
})
export class PollingStation {
  private fb = inject(FormBuilder);
  private readonly transalateService = inject(TranslateService);
  // Signaux de données (Simulés ici, viendraient d'un service)
  departments = signal([
    { id: 1, name: 'ABIDJAN' },
    { id: 2, name: 'DIASPORA' },
  ]);

  allSubPrefectures = signal([
    { id: 10, name: 'ABIDJAN', parentId: 1 },
    { id: 11, name: 'EUROPE', parentId: 2 },
  ]);

  allCommunes = signal([
    { id: 100, name: 'ABOBO', parentId: 10 },
    { id: 101, name: 'COCODY', parentId: 10 },
    { id: 102, name: 'FRANCE', parentId: 11 },
    { id: 103, name: 'YOPOUGON', parentId: 10 },
    { id: 104, name: 'MARCORY', parentId: 10 },
  ]);

  // Modèle du formulaire (Initialisation avec null pour les IDs)
  pollingStationModel = signal({
    departmentId: '',
    subPrefectureId: '',
    communeId: '',
    name: '',
    pollingStationNumber: 1,
  });

  // Définition du formulaire
  pollingStationForm = form(this.pollingStationModel, (schemaPath) => {
    required(schemaPath.departmentId, {
      message: this.transalateService.instant('formError.required'),
    });
    required(schemaPath.subPrefectureId, {
      message: this.transalateService.instant('formError.required'),
    });
    required(schemaPath.communeId, {
      message: this.transalateService.instant('formError.required'),
    });

    required(schemaPath.name, { message: this.transalateService.instant('formError.required') });
    minLength(schemaPath.name, 3, {
      message: this.transalateService.instant('formError.required'),
    });

    required(schemaPath.pollingStationNumber, {
      message: this.transalateService.instant('formError.required'),
    });
    min(schemaPath.pollingStationNumber, 1, {
      message: this.transalateService.instant('formError.positiveNumber'),
    });
  });

  // Signaux calculés pour la cascade (Computed Signals)
  //selectedDeptId = signal<number | null>(null);
  //selectedSpId = signal<number | null>(null);

  // Signaux calculés basés directement sur l'état du formulaire
  selectedDeptId = computed(() => this.pollingStationForm.departmentId().value);
  selectedSpId = computed(() => this.pollingStationForm.subPrefectureId().value);

  filteredSubPrefectures = computed(() => {
    const deptId = Number(this.selectedDeptId());
    return this.allSubPrefectures().filter((sp) => sp.parentId === deptId);
  });

  filteredCommunes = computed(() => {
    const spId = Number(this.selectedSpId());
    return this.allCommunes().filter((c) => c.parentId === spId);
  });

  constructor() {
    effect(() => {
      const deptId = this.selectedDeptId(); // On "écoute" le département
      // Si pas de département, on désactive la SP et on reset
      if (!deptId) {
        this.pollingStationForm.subPrefectureId().disabled;
        //this.pollingStationForm.subPrefectureId().value.set(null);
      }
      //this.pollingStationForm.communeId().value.set(null);
    });

    effect(() => {
      const spId = this.selectedSpId();
      if (!spId) {
        this.pollingStationForm.communeId().disabled;
        //this.pollingStationForm.communeId().value.set(null);
      }
    });
  }

  onSubmit() {
    if (this.pollingStationForm().valid()) {
      const data = this.pollingStationForm().value;
      console.log('Données prêtes pour .Net 10 :', data);
      // Appel vers votre API .NET 10 ici
    }
  }
}
