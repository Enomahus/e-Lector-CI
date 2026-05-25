import { Component, computed, forwardRef, inject, input, output, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { DocumentApiService } from '@app/services/api/document.api.service';
import { TranslateModule } from '@ngx-translate/core';
import { UploadFormValue } from '../upload/upload-form-value';

@Component({
  selector: 'app-file-upload-ui',
  imports: [TranslateModule],
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
    return currentFile?.localFile?.name ?? '';
  });

  // Callbacks pour le ControlValueAssessor
  onChange: (value: UploadFormValue | null) => void = () => {};
  onTouched: () => void = () => {};

  onFileSelected(event: Event): void {
    const files = (event.target as HTMLInputElement).files;
    const selected = files && files.length > 0 ? files[0] : null;
    this.file.set(selected ? { ...this.file(), localFile: selected } : null);

    if (this.onChange) this.onChange(this.file());
    if (this.onTouched) this.onTouched();

    if (this.file()?.localFile) this.fileDropped.emit(this.file()!.localFile!);
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
