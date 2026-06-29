import { Component, effect, forwardRef, inject, input, output, signal } from '@angular/core';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { TranslateModule } from '@ngx-translate/core';
import { DocumentApiService } from '../../services/api/document.api.service';
import { saveBlobAsFile } from '../helpers/document.helper';
import { UploadFormValue } from './upload-form-value';

@Component({
  selector: 'app-upload',
  imports: [MatFormFieldModule, MatInputModule, MatSelectModule, FormsModule, TranslateModule],
  templateUrl: './upload.html',
  styleUrls: ['./upload.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => Upload),
      multi: true,
    },
  ],
})
export class Upload implements ControlValueAccessor {
  allowedExtensions = input<string[]>(['.jpeg', '.pdf']);
  defaultExtension = input<string>('.pdf');
  fileDropped = output<File>();

  private readonly documentService = inject(DocumentApiService);

  // États internes du composant (Signals)
  protected readonly fileName = signal<string>('');
  protected readonly fileExtension = signal<string>('');

  fileInfo?: File[];
  value = signal<UploadFormValue | undefined>(undefined);
  disabled = signal<boolean>(false);

  // Callbacks de ControlValueAccessor
  protected onChange?: (formValue: UploadFormValue | undefined) => void;
  protected onTouched?: () => void;

  constructor() {
    // On initialise l'extension par défaut dès que le input est disponible
    effect(
      () => {
        if (!this.fileExtension()) {
          this.fileExtension.set(this.defaultExtension());
        }
      },
      { allowSignalWrites: true },
    );
  }

  writeValue(val: UploadFormValue | undefined): void {
    this.value.set(val);
    if (val?.remoteFileId) {
      this.documentService.getDocumentInfo(val.remoteFileId).subscribe({
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
    if (val?.localFile) {
      this.fileInfo = [val.localFile];
    }
  }
  registerOnChange(fn: (formValue: UploadFormValue | undefined) => void): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  // 4. Gère l'état activé/désactivé (optionnel mais recommandé)
  setDisabledState(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
  }

  // --- Gestionnaires de mise à jour des états ---

  protected updateFileName(event: File[]): void {
    this.value.set(event.length > 0 ? { ...this.value(), localFile: event[0] } : undefined);
    if (this.onChange) this.onChange(this.value());
    if (this.onTouched) this.onTouched();
    //this.fileName.set(event.length > 0 ? event[0].name : '');
    this.fileDropped.emit(this.value()?.localFile!);
  }

  onClick(event: Event): void {
    //const el = event.target as Element;
    // if (el.closest('.k-upload-button-wrap')) return;

    // const fileEl = el.closest('.k-file');
    // const id = fileEl?.getAttribute('data-uid');

    const id = event.target instanceof HTMLElement ? event.target.getAttribute('data-uid') : null;

    if (id && this.value()?.remoteFileId === id) {
      this.documentService.downloadDocument(id).subscribe({
        next: (file) => {
          saveBlobAsFile(file.data, (this.fileName() ?? '') + this.value()?.localFile?.name);
        },
      });
    } else if (id && this.value()?.localFile) {
      saveBlobAsFile(
        this.value()?.localFile!,
        (this.fileName() ?? '') + this.value()?.localFile?.name,
      );
    }
  }

  protected updateExtension(ext: string): void {
    this.fileExtension.set(ext);
    this.triggerChange();
  }

  // Fusionne le nom et l'extension pour notifier le parent
  private triggerChange(): void {
    const fullName = this.fileName() ? `${this.fileName()}.${this.fileExtension()}` : '';
    this.onChange?.({
      ...this.value(),
      localFile: this.value()?.localFile
        ? new File([this.value()!.localFile!], fullName, { type: this.value()!.localFile!.type })
        : undefined,
    });
  }
}
