import { FormControl, FormGroup, Validators } from '@angular/forms';
import { PollingStationModel } from '@app/services/nswag/api-nswag-client';

export type PollingStationForm = FormGroup<{
  stationNumber: FormControl<string | undefined>;
  wording: FormControl<string | undefined>;
  constituencyId: FormControl<number | undefined>;
  isActive: FormControl<boolean>;
}>;

export function createPollingStationForm(): PollingStationForm {
  return new FormGroup({
    stationNumber: new FormControl<string | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    wording: new FormControl<string | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    constituencyId: new FormControl<number | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    isActive: new FormControl<boolean>(false, {
      validators: Validators.required,
      nonNullable: true,
    }),
  }) as PollingStationForm;
}

export function createPollingStationModelFromForm(form: PollingStationForm): PollingStationModel {
  return {
    stationNumber: form.value.stationNumber,
    wording: form.value.wording,
    constituencyId: form.value.constituencyId,
    isActive: form.value.isActive,
  };
}
