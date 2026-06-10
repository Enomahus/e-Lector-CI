import { Component, HostListener, input, output, signal } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-file-upload',
  imports: [MatIconModule],
  templateUrl: './file-upload.html',
  styleUrl: './file-upload.scss',
})
export class FileUpload {
  label = input.required<string>();
  accept = input<string>('.pdf,.jpg,.jpeg,.png');
  hint = input<string>('PDF, JPG, PNG (max. 5 Mo)');

  fileSelected = output<File | null>();

  isDragging = signal(false);
  file = signal<File | null>(null);

  @HostListener('dragover', ['$event']) onDragOver(event: DragEvent) {
    event.preventDefault();
    this.isDragging.set(true);
  }

  @HostListener('dragleave', ['$event']) onDragLeave(event: DragEvent) {
    event.preventDefault();
    this.isDragging.set(false);
  }

  @HostListener('drop', ['$event']) onDrop(event: DragEvent) {
    event.preventDefault();
    this.isDragging.set(false);
    const droppedFile = event.dataTransfer?.files[0];
    if (droppedFile) this.handleFile(droppedFile);
  }

  onFileSelected(event: any) {
    const selectedFile = event.target.files[0];
    if (selectedFile) this.handleFile(selectedFile);
  }

  handleFile(newFile: File) {
    if (newFile.size > 5 * 1024 * 1024) {
      alert('Fichier trop lourd (5Mo max)');
      return;
    }
    this.file.set(newFile);
    this.fileSelected.emit(newFile); // Remonte au parent (FormGroup)
  }

  clearFile(event: Event) {
    event.stopPropagation();
    this.file.set(null);
    this.fileSelected.emit(null);
  }
}
