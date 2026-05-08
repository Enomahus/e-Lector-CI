import { effect } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Activity } from '@app/pages/types/enumerations';
import { AuthProvider, PersonTitle } from '@app/services/nswag/api-nswag-client';
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
  roles: FormControl<Activity[]>;
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
      roles: new FormControl<Activity[]>(['demandeur'], { nonNullable: true }),
      authProvider: new FormControl<AuthProvider | undefined>({ value: undefined, disabled: true }),
      constituencyId: new FormControl<number | undefined>({ value: undefined, disabled: true }),
    },
    { validators: passwordMatchValidator('password', 'confirmPassword') },
  ) as UserFormFactory;

  const employeeControl = form.controls.employeeNumber;
  const rolesControl = form.controls.roles;
  const updateEmployeeValidators = (roles: Activity[] | null) => {
    const isOnlyDemandeur = roles?.length === 1 && roles[0] === 'demandeur';
    if (!isOnlyDemandeur) {
      employeeControl.setValidators([Validators.required]);
    } else {
      employeeControl.clearValidators();
    }
    employeeControl.updateValueAndValidity({ emitEvent: false });
  };

  updateEmployeeValidators(rolesControl.value);

  rolesControl.valueChanges.subscribe((roles) => {
    updateEmployeeValidators(roles);
  });

  return form;
}

export function setupUserFormFactoryLogic(form: UserFormFactory) {
  const employeeControl = form.controls.employeeNumber;
  const rolesControl = form.controls.roles;

  const rolesSignal = toSignal(rolesControl.valueChanges, { initialValue: rolesControl.value });

  effect(() => {
    const roles = rolesSignal();
    const isOnlyDemandeur = roles.length === 1 && roles[0] === 'demandeur';

    if (!isOnlyDemandeur) {
      employeeControl.setValidators([Validators.required]);
    } else {
      employeeControl.clearValidators();
    }
    employeeControl.updateValueAndValidity({ emitEvent: false });
  });
}
