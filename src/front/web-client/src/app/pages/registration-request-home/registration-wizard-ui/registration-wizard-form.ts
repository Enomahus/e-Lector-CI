import { FormControl, FormGroup, Validators } from '@angular/forms';
import {
  Gender,
  MaritalStatus,
  RegistrationRequestType,
} from '@app/services/nswag/api-nswag-client';

export type RegistrationForm = FormGroup<{
  request: RequestForm;
  identity: IdentityForm;
  filiation: FiliationForm;
  coordinates: CoordinatesForm;
  residence: ResidenceForm;
  requestDocuments: RequestDocumentsForm;
}>;
export type RequestForm = FormGroup<{
  id: FormControl<string | undefined>;
  registrationType: FormControl<RegistrationRequestType | undefined>;
  reasonForRejection: FormControl<string | undefined>;
}>;
export type IdentityForm = FormGroup<{
  gender: FormControl<Gender | undefined>;
  firstName: FormControl<string | undefined>;
  lastName: FormControl<string | undefined>;
  birthDate: FormControl<Date | undefined>;
  birthPlace: FormControl<string | undefined>;
  maritalStatus: FormControl<MaritalStatus | undefined>;
  marriedName: FormControl<string | undefined>;
  nationality: FormControl<string | undefined>;
}>;
export type ParentForm = FormGroup<{
  firstName: FormControl<string | undefined>;
  lastName: FormControl<string | undefined>;
  birthDate: FormControl<Date | undefined>;
  birthPlace: FormControl<string | undefined>;
  nationality: FormControl<string | undefined>;
}>;
export type FiliationForm = FormGroup<{
  fatherId: FormControl<string | undefined>;
  father: ParentForm;
  motherId: FormControl<string | undefined>;
  mother: ParentForm;
}>;
export type CoordinatesForm = FormGroup<{
  profession: FormControl<string | undefined>;
  email: FormControl<string | undefined>;
}>;
export type ResidenceForm = FormGroup<{
  regionId: FormControl<number | undefined>;
  departmentId: FormControl<number | undefined>;
  subPrefectureId: FormControl<number | undefined>;
  municipalityId: FormControl<number | undefined>;
}>;
export type RequestDocumentsForm = FormGroup<{
  registrationCertificateAttachments: FormControl<File | undefined>;
  registrationCniAttachments: FormControl<File | undefined>;
  photoAttachments: FormControl<File | undefined>;
}>;
export function createIdentityForm(): IdentityForm {
  const identity = new FormGroup({
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
  }) as IdentityForm;
  return identity;
}
export function createFiliationForm(): FiliationForm {
  const filisation = new FormGroup({
    fatherId: new FormControl<string | undefined>(undefined, {
      //validators: Validators.required,
      nonNullable: true,
    }),
    father: createParentForm(),
    motherId: new FormControl<string | undefined>(undefined, {
      //validators: Validators.required,
      nonNullable: true,
    }),

    mother: createParentForm(),
  }) as FiliationForm;
  return filisation;
}
export function createCoordinatesForm(): CoordinatesForm {
  const coordinates = new FormGroup({
    profession: new FormControl<string | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    email: new FormControl<string | undefined>(undefined, {
      nonNullable: true,
    }),
  }) as CoordinatesForm;
  return coordinates;
}
export function createResidenceForm(): ResidenceForm {
  const residence = new FormGroup({
    regionId: new FormControl<number | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    departmentId: new FormControl<number | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    subPrefectureId: new FormControl<number | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    municipalityId: new FormControl<number | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
  }) as ResidenceForm;
  return residence;
}
export function createRequestDocumentsForm(): RequestDocumentsForm {
  const requestDocuments = new FormGroup({
    registrationCertificateAttachments: new FormControl<File | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    registrationCniAttachments: new FormControl<File | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    photoAttachments: new FormControl<File | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
  }) as RequestDocumentsForm;
  return requestDocuments;
}
export function createRegistrationForm(): RegistrationForm {
  const form = new FormGroup({
    request: createRequestForm(),
    identity: createIdentityForm(),
    filiation: createFiliationForm(),
    coordinates: createCoordinatesForm(),
    residence: createResidenceForm(),
    requestDocuments: createRequestDocumentsForm(),
  }) as RegistrationForm;
  return form;
}
export function createParentForm(): ParentForm {
  return new FormGroup({
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
    nationality: new FormControl<string | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
  }) as ParentForm;
}
export function createRequestForm(): RequestForm {
  return new FormGroup({
    id: new FormControl<string | undefined>(undefined, { nonNullable: true }),
    registrationType: new FormControl<RegistrationRequestType | undefined>(undefined, {
      validators: Validators.required,
      nonNullable: true,
    }),
    reasonForRejection: new FormControl<string | undefined>(undefined, {
      validators: Validators.maxLength(500),
      nonNullable: true,
    }),
  }) as RequestForm;
}
