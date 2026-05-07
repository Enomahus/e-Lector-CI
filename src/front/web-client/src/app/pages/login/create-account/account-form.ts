import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthProvider, PersonTitle } from '@app/services/nswag/api-nswag-client';
import { passwordMatchValidator } from '@app/shared/helpers/form.helper';
import { phoneNumberValidator } from '@app/shared/phone-input/phone-input-intl.validator';
import { Activity } from './create-account';

export type AccountForm = FormGroup<{
  civility: FormControl<PersonTitle>;
  lastName: FormControl<string | undefined>;
  firstName: FormControl<string | undefined>;
  phone: FormControl<string | undefined>;
  email: FormControl<string | undefined>;
  password: FormControl<string | undefined>;
  confirmPassword: FormControl<string | undefined>;
  employeeNumber: FormControl<string | undefined>;
  roles: FormControl<Activity[]>;
  authProvider: FormControl<AuthProvider | undefined>;
}>;

export function createAccountForm(): AccountForm {
  const form = new FormGroup(
    {
      civility: new FormControl<PersonTitle>('mr'),
      lastName: new FormControl<string | undefined>(undefined, [Validators.required]),
      firstName: new FormControl<string | undefined>(undefined, [Validators.required]),
      employeeNumber: new FormControl<string | undefined>(undefined),
      phone: new FormControl<string | undefined>(undefined, [
        Validators.required,
        phoneNumberValidator(),
      ]),
      email: new FormControl<string | undefined>(undefined, [
        Validators.required,
        Validators.email,
      ]),
      password: new FormControl<string | undefined>(undefined, [Validators.required]),
      confirmPassword: new FormControl<string | undefined>(undefined, [Validators.required]),
      roles: new FormControl<Activity[]>(['demandeur']),
      authProvider: new FormControl<AuthProvider | undefined>(undefined),
    },
    { validators: passwordMatchValidator('password', 'confirmPassword') },
  ) as AccountForm;

  const employeeControl = form.controls.employeeNumber;
  const rolesControl = form.controls.roles;

  const updateEmployeeValidators = (roles: Activity[] | null) => {
    const isOnlyDemandeur = roles?.length === 1 && roles[0] === 'demandeur';

    if (!isOnlyDemandeur) {
      employeeControl.setValidators([Validators.required]);
    } else {
      employeeControl.clearValidators();
    }

    employeeControl.updateValueAndValidity();
  };

  updateEmployeeValidators(rolesControl.value);

  rolesControl.valueChanges.subscribe((roles) => {
    updateEmployeeValidators(roles);
  });

  form.controls.authProvider.disable();

  return form;
}
