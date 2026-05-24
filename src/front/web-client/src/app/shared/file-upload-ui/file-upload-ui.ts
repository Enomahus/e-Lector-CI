import { Component, computed, forwardRef, input, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-file-upload-ui',
  imports: [],
  templateUrl: './file-upload-ui.html',
  styleUrls: ['./file-upload-ui.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => FileUploadUi),
      multi: true,
    },
  ],
})
export class FileUploadUi implements ControlValueAccessor {
  accepts = input<string>('.pdf');

  //fileName = signal<string>('');
  isDisabled = signal<boolean>(false);
  file = signal<File | null>(null);

  fileName = computed(() => {
    const currentFile = this.file();
    return currentFile ? currentFile.name : '';
  });

  // Callbacks pour le ControlValueAssessor
  onChange: (value: File | null) => void = () => {};
  onTouch: () => void = () => {};

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const selectedFile = input.files[0];
      this.file.set(selectedFile);

      this.onChange(this.file());
      this.onTouch();
    }
  }

  writeValue(value: File | null): void {
    this.file.set(value || null);
  }
  registerOnChange(fn: (value: File | null) => void): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }
  setDisabledState(isDisabled: boolean): void {
    this.isDisabled.set(isDisabled);
  }
}
