import { FormControl, FormGroup, Validators } from '@angular/forms';

export type PollingStationForm = FormGroup<{
  stationNumber: FormControl<string | undefined>;
  wording: FormControl<string | undefined>;
  constituencyId: FormControl<number | undefined>;
  isActive: FormControl<boolean>;
}>;

export function createPollingStationForm(fromCreate: boolean): PollingStationForm {
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

  form.controls.stationNumber.valueChanges.subscribe(() =>
    updateStationNumberValidators(form, fromCreate),
  );

  return form;
}

export function updateStationNumberValidators(form: PollingStationForm, isCreate: boolean) {
  const stationNumberControl = form.controls.stationNumber;

  if (!isCreate) {
    stationNumberControl.disable();
  } else {
    stationNumberControl.clearValidators();
  }
  stationNumberControl.updateValueAndValidity({ emitEvent: false });
}
