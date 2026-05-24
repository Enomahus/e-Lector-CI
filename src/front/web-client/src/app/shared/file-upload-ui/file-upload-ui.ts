import { Component, computed, forwardRef, inject, input, output, signal } from '@angular/core';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from '@angular/forms';
import { DocumentApiService } from '@app/services/api/document.api.service';
import { TranslateModule } from '@ngx-translate/core';
import { UploadFormValue } from '../upload/upload-form-value';

@Component({
  selector: 'app-file-upload-ui',
  imports: [FormsModule, TranslateModule],
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
  fileDropped = output<File>();

  private readonly documentService = inject(DocumentApiService);

  isDisabled = signal<boolean>(false);
  file = signal<UploadFormValue | null>(null);
  fileInfo?: File[];

  fileName = computed(() => {
    const currentFile = this.file();
    return currentFile ? currentFile.localFile : '';
  });

  // Callbacks pour le ControlValueAssessor
  onChange: (value: UploadFormValue | null) => void = () => {};
  onTouched: () => void = () => {};

  onFileSelected(event: File[]): void {
    this.file.set(event.length > 0 ? { ...this.file(), localFile: event[0] } : null);

    if (this.onChange) this.onChange(this.file());
    if (this.onTouched) this.onTouched();

    this.fileDropped.emit(this.file()?.localFile!);
  }

  writeValue(value: UploadFormValue | null): void {
    this.file.set(value || null);
    if (value?.remoteFileId) {
      this.documentService.getDocumentInfo(value.remoteFileId).subscribe({
        next: (doc) => {
          this.fileInfo = [
            {
              name: doc.data?.documentInfo?.fileName,
              size: doc.data?.documentInfo?.size,
              type: doc.data?.documentInfo?.contentType,
            } as File,
          ];
        },
      });
    }

    if (value?.localFile) {
      this.fileInfo = [value.localFile];
    }
  }

  registerOnChange(fn: (formValue: UploadFormValue | null) => void): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }
  setDisabledState(isDisabled: boolean): void {
    this.isDisabled.set(isDisabled);
  }
}
