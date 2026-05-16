import { FormControl, FormGroup, Validators } from '@angular/forms';

export type PollingStationForm = FormGroup<{
  stationNumber: FormControl<string | undefined>;
  wording: FormControl<string | undefined>;
  constituencyId: FormControl<number | undefined>;
  isActive: FormControl<boolean>;
}>;

export function createPollingStationForm(isToCreate: boolean): PollingStationForm {
  const form = new FormGroup({
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

  if (!isToCreate) {
    form.controls.stationNumber.disable();
  } else {
    form.controls.stationNumber.clearValidators();
  }
  form.controls.stationNumber.updateValueAndValidity({ emitEvent: false });

  return form;
}
