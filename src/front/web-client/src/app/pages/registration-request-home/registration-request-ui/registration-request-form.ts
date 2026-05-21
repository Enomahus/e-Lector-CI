import { FormControl, FormGroup, Validators } from '@angular/forms';
import {
  Gender,
  MaritalStatus,
  RegistrationRequestType,
} from '@app/services/nswag/api-nswag-client';
import { UploadFormValue } from '@app/shared/upload/upload-form-value';

export type RegistrationRequestForm = FormGroup<{
  id: FormControl<string | undefined>;
  request: RequestsForm;
  citizen: CitizenForm;
  requestDocuments: RequestDocumentsForm;
}>;

export type RequestsForm = FormGroup<{
  constituencyId: FormControl<number | undefined>;
  registrationType: FormControl<RegistrationRequestType | undefined>;
  reasonForRejection: FormControl<string | undefined>;
}>;

export type RequestDocumentsForm = FormGroup<{
  registrationCertificateAttachments: FormControl<UploadFormValue | undefined>;
  registrationCniAttachments: FormControl<UploadFormValue | undefined>;
  photoAttachments: FormControl<UploadFormValue | undefined>;
}>;

export type CitizenForm = FormGroup<{
  gender: FormControl<Gender | undefined>;
  firstName: FormControl<string | undefined>;
  lastName: FormControl<string | undefined>;
  birthDate: FormControl<Date | undefined>;
  birthPlace: FormControl<string | undefined>;
  maritalStatus: FormControl<MaritalStatus | undefined>;
  marriedName: FormControl<string | undefined>;
  nationality: FormControl<string | undefined>;
  profession: FormControl<string | undefined>;
  email: FormControl<string | undefined>;
  physicalAddress: FormControl<string | undefined>;
  postalAddress: FormControl<string | undefined>;
  fatherId: FormControl<string | undefined>;
  motherId: FormControl<string | undefined>;
}>;

export function createRequestsForm(): RequestsForm {
  const form = new FormGroup({
    constituencyId: new FormControl<number | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    registrationType: new FormControl<RegistrationRequestType | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    reasonForRejection: new FormControl<string | undefined>(undefined, {
      validators: Validators.maxLength(500),
      nonNullable: true,
    }),
  }) as RequestsForm;
  return form;
}

export function createCitizenForm(): CitizenForm {
  const form = new FormGroup({
    gender: new FormControl<Gender | undefined>(undefined, {
      validators: [Validators.required],
      nonNullable: true,
    }),
    firstName: new FormControl<string | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    lastName: new FormControl<string | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    birthDate: new FormControl<Date | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    birthPlace: new FormControl<string | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    maritalStatus: new FormControl<MaritalStatus | undefined>(undefined, {
      validators: [Validators.required],
      nonNullable: true,
    }),
    marriedName: new FormControl<string | undefined>(undefined),
    nationality: new FormControl<string | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    profession: new FormControl<string | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    email: new FormControl<string | undefined>(undefined, {
      nonNullable: true,
    }),
    physicalAddress: new FormControl<string | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    postalAddress: new FormControl<string | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    fatherId: new FormControl<string | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    motherId: new FormControl<string | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
  }) as CitizenForm;
  return form;
}

export function createRequestDocumentsForm(): RequestDocumentsForm {
  const form = new FormGroup({
    registrationCertificateAttachments: new FormControl<UploadFormValue | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    registrationCniAttachments: new FormControl<UploadFormValue | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    photoAttachments: new FormControl<UploadFormValue | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
  }) as RequestDocumentsForm;
  return form;
}

export function createRegistrationRequestForm(): RegistrationRequestForm {
  const form = new FormGroup({
    id: new FormControl<string | undefined>(undefined, { nonNullable: true }),
    request: createRequestsForm(),
    citizen: createCitizenForm(),
    requestDocuments: createRequestDocumentsForm(),
  }) as RegistrationRequestForm;
  return form;
}
