import {
  Gender,
  LocationLevel,
  MaritalStatus,
  PersonTitle,
  RegistrationRequestDocumentType,
  RegistrationRequestType,
} from '@app/services/nswag/api-nswag-client';

export const allLocationLevel: LocationLevel[] = [
  'region',
  'department',
  'subPrefecture',
  'municipality',
  'votingLocation',
];

export const allRegistrationRequestType: RegistrationRequestType[] = [
  'registrationRequest',
  'registrationDataUpdate',
];

export const allGenders: Gender[] = ['m', 'f'];
export const allMaritalStatus: MaritalStatus[] = ['single', 'married', 'divorced', 'widowed'];
export const allPersonTitle: PersonTitle[] = ['mr', 'ms', 'mrs'];
export const allRegistrationDocumentType: RegistrationRequestDocumentType[] = [
  'identityDocumentOrNationalCertificate',
  'photo',
];
