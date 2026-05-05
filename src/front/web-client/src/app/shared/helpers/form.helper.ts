import { AbstractControl, ValidatorFn } from '@angular/forms';

export function passwordMatchValidator(password: string, confirmPassword: string): ValidatorFn {
  return (control: AbstractControl): Record<string, boolean> | null => {
    const passwordValue = control.get(password)?.value;
    const confirmPasswordValue = control.get(confirmPassword)?.value;

    if (!passwordValue || !confirmPasswordValue) return null;

    const isMatch = passwordValue === confirmPasswordValue;
    return isMatch ? null : { passwordMismatch: true };
  };
}
