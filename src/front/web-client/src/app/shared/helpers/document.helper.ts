export function saveBlobAsFile(blob: Blob, fileName?: string): void {
  const url = window.URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = fileName ?? 'DOCUMENT_NAME_MISSING';
  document.body.appendChild(a); // Nécessaire pour Firefox
  a.click();
  document.body.removeChild(a); // Nettoyage après le clic
  window.URL.revokeObjectURL(url);
}
