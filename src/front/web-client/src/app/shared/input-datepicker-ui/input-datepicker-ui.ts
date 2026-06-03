import { Component, forwardRef, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-input-datepicker-ui',
  imports: [],
  templateUrl: './input-datepicker-ui.html',
  styleUrl: './input-datepicker-ui.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => InputDatepickerUi),
      multi: true,
    },
  ],
})
export class InputDatepickerUi implements ControlValueAccessor {
  readonly value = signal<string>('');
  readonly id = signal<string>('');
  readonly disabled = signal<boolean>(false);

  onChange: (value: string) => void = () => {};
  onTouched: () => void = () => {};

  writeValue(value: string): void {
    this.value.set(value || '');
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
  }

  // Méthode appelée à chaque saisie de l'utilisateur
  onInputChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.value.set(input.value);
    this.onChange(input.value); // Notifie le formulaire parent du changement
  }
}
