import { FileParameter } from '../../services/nswag/api-nswag-client';

export async function getFileParameters(values?: File[]): Promise<FileParameter[]> {
  if (!values) return [];
  const attachments = await Promise.all(
    values
      .filter((f) => f instanceof File)
      .map((f) => f as File)
      .map(
        async (file) =>
          ({
            fileName: file.name,
            data: new Blob([await file.arrayBuffer()], { type: file.type }),
          }) as FileParameter,
      ),
  );
  return attachments;
}

export async function getFileParameter(
  values?: File | undefined,
): Promise<FileParameter | undefined> {
  if (!values || !(values instanceof File)) return undefined;

  const attachments = {
    fileName: values.name,
    data: new Blob([await values.arrayBuffer()], { type: values.type }),
  } as FileParameter;

  return attachments;
}
