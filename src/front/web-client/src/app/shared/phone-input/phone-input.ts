import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from '@angular/forms';
import { CountryISO } from '@app/enums/country-iso.enum';
import { PhoneNumberUtil } from 'google-libphonenumber';
import { NgxIntlTelInput } from '../ngx-intl-tel-input/ngx-intl-tel-input';
import { ChangeData } from './interfaces/change-data';

const phoneUtil = PhoneNumberUtil.getInstance();

@Component({
  selector: 'app-phone-input',
  imports: [NgxIntlTelInput, FormsModule],
  templateUrl: './phone-input.html',
  styleUrl: './phone-input.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => PhoneInput),
      multi: true,
    },
  ],
})
export class PhoneInput implements ControlValueAccessor {
  // phoneNumber = signal('');
  // selectedCountry = signal('CI');
  // disabled = signal(false);
  // required = input.required<boolean>();

  // countries = [
  //   { name: 'France', code: 'FR', dial: '33', flag: '🇫🇷' },
  //   { name: 'Belgique', code: 'BE', dial: '32', flag: '🇧🇪' },
  //   { name: 'Suisse', code: 'CH', dial: '41', flag: '🇨🇭' },
  // ];

  // onChange = (value: string) => {};
  // onTouched = () => {};

  // writeValue(value: string): void {
  //   if (value) {
  //     try {
  //       const parsed = phoneUtil.parseAndKeepRawInput(value);
  //       const regionCode = phoneUtil.getRegionCodeForNumber(parsed);
  //       if (regionCode) {
  //         this.selectedCountry.set(regionCode);
  //         this.phoneNumber.set(phoneUtil.format(parsed, PhoneNumberFormat.NATIONAL));
  //       }
  //     } catch (e) {
  //       this.phoneNumber.set(value);
  //     }
  //   } else {
  //     this.phoneNumber.set('');
  //   }
  // }

  // registerOnChange(fn: any): void {
  //   this.onChange = fn;
  // }
  // registerOnTouched(fn: any): void {
  //   this.onTouched = fn;
  // }
  // setDisabledState(isDisabled: boolean): void {
  //   this.disabled.set(isDisabled);
  // }

  // onCountryChange(event: any): void {
  //   this.selectedCountry.set(event.target.value);
  //   this.triggerChange();
  // }

  // onNumberInput(event: any): void {
  //   this.phoneNumber.set(event.target.value);
  //   this.triggerChange();
  // }

  // private triggerChange(): void {
  //   const rawInput = this.phoneNumber();
  //   if (!rawInput) {
  //     this.onChange('');
  //     return;
  //   }

  //   try {
  //     const parsed = phoneUtil.parseAndKeepRawInput(rawInput, this.selectedCountry());
  //     if (phoneUtil.isValidNumberForRegion(parsed, this.selectedCountry())) {
  //       // On renvoie le format international au parent (+33...)
  //       const e164 = phoneUtil.format(parsed, PhoneNumberFormat.E164);
  //       this.onChange(e164);
  //     } else {
  //       // Si invalide, on peut renvoyer la valeur brute ou null
  //       this.onChange(rawInput);
  //     }
  //   } catch (e) {
  //     this.onChange(rawInput);
  //   }
  // }

  // getDialCode(): string {
  //   const country = this.countries.find((c) => c.code === this.selectedCountry());
  //   return country ? country.dial : '';
  // }

  @Input()
  preferredCountries: CountryISO[] = ['fr'];

  @Input()
  selectFirstCountry = false;

  @Input()
  maxLength = 15;

  @Input()
  required = true;

  @Input({ required: true }) labelForId!: string;

  @Input()
  // This needs to be changed if multiple phone inputs are present at the same time.
  name: string = 'phone';

  selectedCountryISO: CountryISO = 'fr';
  disabled = false;
  value?: string;

  onChange = (_?: string) => {};
  onTouched = () => {};

  writeValue(obj?: string): void {
    this.value = obj;
  }
  registerOnChange(fn: (_?: string) => {}): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: () => {}): void {
    this.onTouched = fn;
  }
  setDisabledState?(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  mapChange(ev?: ChangeData): void {
    if (ev && this.value !== ev?.internationalNumber) {
      this.writeValue(ev?.internationalNumber);
      this.onChange(ev?.internationalNumber);
    }
  }
}
