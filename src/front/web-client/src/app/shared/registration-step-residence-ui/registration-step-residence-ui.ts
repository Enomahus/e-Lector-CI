import {
  Component,
  computed,
  DestroyRef,
  inject,
  input,
  OnInit,
  output,
  signal,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ResidenceForm } from '../../pages/registration-request-home/registration-wizard-ui/registration-wizard-form';
import { ConstituencyApiService } from '../../services/api/constituency.api.service';
import { GetConstituenciesResponse } from '../../services/nswag/api-nswag-client';

@Component({
  selector: 'app-registration-step-residence-ui',
  imports: [ReactiveFormsModule, TranslateModule],
  templateUrl: './registration-step-residence-ui.html',
  styleUrl: './registration-step-residence-ui.scss',
})
export class RegistrationStepResidenceUi implements OnInit {
  stepForm = input.required<ResidenceForm>();
  municipalityChange = output<number | null>();

  private readonly constituencieService = inject(ConstituencyApiService);
  private readonly destroyRef = inject(DestroyRef);
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
    this.setupFormLinkage();
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

        // Application de l'état d'activation initial (Utile pour le rechargement de données / mode édition)
        this.toggleControlStates();
      },
    });
  }
  private setupFormLinkage(): void {
    const formControls = this.stepForm().controls;

    // Écoute de la Région
    formControls.regionId.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((value) => {
        this.selectedRegionId.set(value ? Number(value) : null);

        // Reset en cascade des valeurs enfants sans propager d'événements de boucle
        formControls.departmentId.setValue(undefined, { emitEvent: false });
        formControls.subPrefectureId.setValue(undefined, { emitEvent: false });
        formControls.municipalityId.setValue(undefined, { emitEvent: false });

        this.selectedDepartmentId.set(null);
        this.selectedSubPrefectureId.set(null);

        this.municipalityChange.emit(null);
        this.toggleControlStates();
        //formControls.departmentId.setValue(undefined);
      });

    // Écoute du Département
    formControls.departmentId.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((value) => {
        this.selectedDepartmentId.set(value ? Number(value) : null);

        formControls.subPrefectureId.setValue(undefined, { emitEvent: false });
        formControls.municipalityId.setValue(undefined, { emitEvent: false });

        this.selectedSubPrefectureId.set(null);

        this.municipalityChange.emit(null);
        this.toggleControlStates();

        //this.selectedDepartmentId.set(value ? Number(value) : null);
        //formControls.subPrefectureId.setValue(undefined);
      });

    // Écoute de la Sous-Préfecture
    formControls.subPrefectureId.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((value) => {
        this.selectedSubPrefectureId.set(value ? Number(value) : null);
        formControls.municipalityId.setValue(undefined, { emitEvent: false });

        this.municipalityChange.emit(null);
        this.toggleControlStates();
      });

    // 3. Écoute spécifique de la Municipalité pour émettre vers le parent
    formControls.municipalityId.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((value) => {
        const municipalityId = value ? Number(value) : null;
        this.municipalityChange.emit(municipalityId);
      });
  }

  private toggleControlStates(): void {
    const controls = this.stepForm().controls;

    // 1. Département actif ssi Région renseignée
    if (controls.regionId.value) {
      controls.departmentId.enable({ emitEvent: false });
    } else {
      controls.departmentId.disable({ emitEvent: false });
    }

    // 2. Sous-Préfecture active ssi Département renseigné et actif
    if (controls.departmentId.value && controls.departmentId.enabled) {
      controls.subPrefectureId.enable({ emitEvent: false });
    } else {
      controls.subPrefectureId.disable({ emitEvent: false });
    }

    // 3. Municipalité active ssi Sous-Préfecture renseignée et active
    if (controls.subPrefectureId.value && controls.subPrefectureId.enabled) {
      controls.municipalityId.enable({ emitEvent: false });
    } else {
      controls.municipalityId.disable({ emitEvent: false });
    }
  }
}
