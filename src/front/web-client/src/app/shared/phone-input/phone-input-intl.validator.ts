import { AbstractControl, ValidationErrors } from '@angular/forms';
import { PhoneNumberUtil } from 'google-libphonenumber';

const phoneUtil = PhoneNumberUtil.getInstance();

export function phoneNumberValidator() {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;

    try {
      const phoneNumber = phoneUtil.parse(control.value);
      return phoneUtil.isValidNumber(phoneNumber) ? null : { pattern: true };
    } catch (e) {
      return { pattern: true };
    }
  };
}
