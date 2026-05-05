import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthProvider, PersonTitle } from '@app/services/nswag/api-nswag-client';
import { passwordMatchValidator } from '@app/shared/helpers/form.helper';
import { Activity } from './create-account';

export type AccountForm = FormGroup<{
  civility: FormControl<PersonTitle>;
  lastName: FormControl<string | undefined>;
  firstName: FormControl<string | undefined>;
  phone: FormControl<string | undefined>;
  email: FormControl<string | undefined>;
  password: FormControl<string | undefined>;
  confirmPassword: FormControl<string | undefined>;
  emplyeeNumber: FormControl<string | undefined>;
  roles: FormControl<Activity[]>;
  authProvider: FormControl<AuthProvider | undefined>;
}>;

export function createAccountForm(isRegistration = false): AccountForm {
  const form = new FormGroup(
    {
      civility: new FormControl<PersonTitle>('mr'),
      lastName: new FormControl<string | undefined>(undefined),
      firstName: new FormControl<string | undefined>(undefined),
      emplyeeNumber: new FormControl<string | undefined>(undefined),
      phone: new FormControl<string | undefined>(undefined),
      email: new FormControl<string | undefined>(undefined),
      password: new FormControl<string | undefined>(undefined),
      confirmPassword: new FormControl<string | undefined>(undefined),
      roles: new FormControl<Activity[]>(['demandeur']),
      authProvider: new FormControl<AuthProvider | undefined>(undefined),
    },
    { validators: passwordMatchValidator('password', 'confirmPassword') },
  ) as AccountForm;

  if (isRegistration) {
    form.controls.emplyeeNumber.setValidators(Validators.requiredTrue);
  }
  form.controls.authProvider.setValue('email');
  form.controls.authProvider.disable();

  return form;
}
