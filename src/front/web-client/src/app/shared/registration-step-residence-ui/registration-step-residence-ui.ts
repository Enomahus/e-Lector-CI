import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { ResidenceForm } from '@app/pages/registration-request-home/registration-wizard-ui/registration-wizard-form';
import { ConstituencyApiService } from '@app/services/api/constituency.api.service';
import { GetConstituenciesResponse } from '@app/services/nswag/api-nswag-client';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-registration-step-residence-ui',
  imports: [ReactiveFormsModule, TranslateModule],
  templateUrl: './registration-step-residence-ui.html',
  styleUrl: './registration-step-residence-ui.scss',
})
export class RegistrationStepResidenceUi implements OnInit {
  stepForm = input.required<ResidenceForm>();

  private readonly constituencieService = inject(ConstituencyApiService);
  constituencies = signal<GetConstituenciesResponse[] | null>(null);

  allRegion = signal<GetConstituenciesResponse[] | null>(null);
  selectedRegionId = signal<number | null>(null);
  selectedDepartmentId = signal<number | null>(null);
  selectedSubPrefectureId = signal<number | null>(null);

  allDepartement = computed<GetConstituenciesResponse[]>(() => {
    const regionId = this.selectedRegionId();
    if (!regionId) return [];

    const selectedRegion = this.allRegion()?.find((r) => r.id === regionId);
    return selectedRegion?.children ?? [];
  });

  allSubPrefectures = computed<GetConstituenciesResponse[]>(() => {
    const departmentId = this.selectedDepartmentId();
    if (!departmentId) return [];

    const selectedDepartment = this.allDepartement()?.find((d) => d.id === departmentId);
    return selectedDepartment?.children ?? [];
  });

  allMunicipalities = computed<GetConstituenciesResponse[]>(() => {
    const subPrefectureId = this.selectedSubPrefectureId();
    if (!subPrefectureId) return [];

    const selectedSubPrefecture = this.allSubPrefectures()?.find((sp) => sp.id === subPrefectureId);
    return selectedSubPrefecture?.children ?? [];
  });

  ngOnInit(): void {
    this.loadConstituencies();

    const formControls = this.stepForm().controls;

    // Écoute de la Région
    formControls.regionId.valueChanges.subscribe((value) => {
      this.selectedRegionId.set(value ? Number(value) : null);
      formControls.departmentId.setValue(undefined);
    });

    // Écoute du Département
    formControls.departmentId.valueChanges.subscribe((value) => {
      this.selectedDepartmentId.set(value ? Number(value) : null);
      formControls.subPrefectureId.setValue(undefined);
    });

    // Écoute de la Sous-Préfecture
    formControls.subPrefectureId.valueChanges.subscribe((value) => {
      this.selectedSubPrefectureId.set(value ? Number(value) : null);
      formControls.municipalityId.setValue(undefined);
    });
  }

  private loadConstituencies(): void {
    this.constituencieService.getConstituencyTree({}).subscribe({
      next: (response) => {
        this.constituencies.set(response.data!);
        const data = this.constituencies()?.filter((d) => d.level === 'region');
        this.allRegion.set(data!);

        const controls = this.stepForm().controls;
        if (controls.regionId.value) this.selectedRegionId.set(Number(controls.regionId.value));

        if (controls.departmentId.value)
          this.selectedDepartmentId.set(Number(controls.departmentId.value));

        if (controls.subPrefectureId.value)
          this.selectedSubPrefectureId.set(Number(controls.subPrefectureId.value));
      },
    });
  }
}
