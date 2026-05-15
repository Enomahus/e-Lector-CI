import { FormControl, FormGroup, Validators } from '@angular/forms';
import { LocationLevel } from '@app/services/nswag/api-nswag-client';

export type ConstituencyForm = FormGroup<{
  code: FormControl<string>;
  wording: FormControl<string>;
  level: FormControl<LocationLevel>;
  parentId: FormControl<number | undefined>;
  isActive: FormControl<boolean>;
}>;

export function createConstituencyForm(): ConstituencyForm {
  return new FormGroup({
    code: new FormControl<string>('', { validators: Validators.required, nonNullable: true }),
    wording: new FormControl<string>('', { validators: Validators.required, nonNullable: true }),
    level: new FormControl<LocationLevel>('votingLocation', {
      validators: Validators.required,
      nonNullable: true,
    }),
    parentId: new FormControl<number | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: false,
    }),
    isActive: new FormControl<boolean>(true, {
      validators: Validators.required,
      nonNullable: true,
    }),
  }) as ConstituencyForm;
}
