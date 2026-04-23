import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { form, FormField, min, minLength, required } from '@angular/forms/signals';
import { PollingStationApiService } from '@app/services/api/polling-station.api.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-polling-station',
  imports: [CommonModule, FormField, ReactiveFormsModule],
  templateUrl: './polling-station.html',
  styleUrl: './polling-station.scss',
})
export class PollingStation implements OnInit {
  private fb = inject(FormBuilder);
  private readonly transalateService = inject(TranslateService);
  private readonly pollingStationApiService = inject(PollingStationApiService);

  // Modèle du formulaire (Initialisation avec null pour les IDs)
  pollingStationModel = signal({
    stationNumber: '',
    wording: '',
    constituencyId: 0,
    isActive: false,
  });

  // Définition du formulaire
  pollingStationForm = form(this.pollingStationModel, (schemaPath) => {
    required(schemaPath.isActive, {
      message: this.transalateService.instant('formError.required'),
    });

    required(schemaPath.constituencyId, {
      message: this.transalateService.instant('formError.required'),
    });
    min(schemaPath.constituencyId, 1, {
      message: this.transalateService.instant('formError.positiveNumber'),
    });

    required(schemaPath.wording, { message: this.transalateService.instant('formError.required') });
    minLength(schemaPath.wording, 3, {
      message: this.transalateService.instant('formError.minLength'),
    });

    required(schemaPath.stationNumber, {
      message: this.transalateService.instant('formError.required'),
    });
    min(schemaPath.stationNumber, 1, {
      message: this.transalateService.instant('formError.positiveNumber'),
    });
  });

  ngOnInit(): void {
    // this.pollingStationApiService.getPollingStations().subscribe((response) => {
    //   console.log('Polling stations for constituency 1:', response);
    // });
  }

  onSubmit() {
    if (this.pollingStationForm().valid()) {
      const data = this.pollingStationForm().value;
      console.log('Données prêtes pour .Net 10 :', data);
    }
  }
}
