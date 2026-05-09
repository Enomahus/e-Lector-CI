import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthProvider, PersonTitle, UserType } from '@app/services/nswag/api-nswag-client';
import { passwordMatchValidator } from '../helpers/form.helper';
import { phoneNumberValidator } from '../phone-input/phone-input-intl.validator';

export type UserFormFactory = FormGroup<{
  civility: FormControl<PersonTitle>;
  lastName: FormControl<string | undefined>;
  firstName: FormControl<string | undefined>;
  phone: FormControl<string | undefined>;
  email: FormControl<string | undefined>;
  password: FormControl<string | undefined>;
  confirmPassword: FormControl<string | undefined>;
  employeeNumber: FormControl<string | undefined>;
  userType: FormControl<UserType[]>;
  roles: FormControl<string[]>;
  authProvider: FormControl<AuthProvider | undefined>;
  constituencyId: FormControl<number | undefined>;
}>;

export function createUserForm(): UserFormFactory {
  const form = new FormGroup(
    {
      civility: new FormControl<PersonTitle>('mr', { nonNullable: true }),
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
      userType: new FormControl<UserType[]>(['none'], { nonNullable: true }),
      roles: new FormControl<string[]>([''], { nonNullable: true }),
      authProvider: new FormControl<AuthProvider | undefined>({ value: undefined, disabled: true }),
      constituencyId: new FormControl<number | undefined>({ value: undefined, disabled: true }),
    },
    { validators: passwordMatchValidator('password', 'confirmPassword') },
  ) as UserFormFactory;

  form.controls.roles.valueChanges.subscribe((roles) => updateEmployeeValidators(form));

  return form;
}

export function updateEmployeeValidators(form: UserFormFactory) {
  const employeeControl = form.controls.employeeNumber;
  const rolesControl = form.controls.roles;

  const isOnlyDemandeur =
    Array.isArray(rolesControl.value) &&
    rolesControl.value.length === 1 &&
    rolesControl.value[0] === 'requester';

  if (!isOnlyDemandeur) {
    employeeControl.setValidators([Validators.required]);
  } else {
    employeeControl.clearValidators();
  }
  employeeControl.updateValueAndValidity({ emitEvent: false });
}
