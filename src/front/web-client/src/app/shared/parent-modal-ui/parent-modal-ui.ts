import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { allGenders } from '../../pages/types/enumerations';
import { CitizenApiService } from '../../services/api/citizen.api.service';
import {
  CreateBasicCitizenCommand,
  Gender,
  GetCitizensResponse,
} from '../../services/nswag/api-nswag-client';
import { InputDatepickerUi } from '../input-datepicker-ui/input-datepicker-ui';
import { Loader } from '../loader/loader';

@Component({
  selector: 'app-parent-modal-ui',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    TranslateModule,
    InputDatepickerUi,
    Loader,
  ],
  templateUrl: './parent-modal-ui.html',
  styleUrl: './parent-modal-ui.scss',
})
export class ParentModalUi {
  private readonly dialogRef = inject(MatDialogRef<ParentModalUi>);
  protected readonly genderData: Gender = inject(MAT_DIALOG_DATA);
  private readonly citizenService = inject(CitizenApiService);

  protected isSaving = signal<boolean>(false);

  allGenders = allGenders;

  protected parentForm = new FormGroup({
    id: new FormControl<string | undefined>(undefined),
    gender: new FormControl<Gender>(this.genderData, { nonNullable: true }),
    firstName: new FormControl<string>('', {
      validators: [Validators.required],
      nonNullable: true,
    }),
    lastName: new FormControl<string>('', { validators: [Validators.required], nonNullable: true }),
    birthDate: new FormControl<Date | undefined>(undefined, { validators: [Validators.required] }),
    birthPlace: new FormControl<string>('', {
      validators: [Validators.required],
      nonNullable: true,
    }),
    nationality: new FormControl<string>('', {
      validators: [Validators.required],
      nonNullable: true,
    }),
  });

  protected onSave(): void {
    if (this.parentForm.invalid) {
      this.parentForm.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);
    const payload: CreateBasicCitizenCommand = {
      birthDate: this.parentForm.value.birthDate
        ? new Date(this.parentForm.value.birthDate).toISOString()
        : undefined,
      birthPlace: this.parentForm.value.birthPlace,
      nationality: this.parentForm.value.nationality,
      lastName: this.parentForm.value.lastName,
      firstName: this.parentForm.value.firstName,
      gender: this.parentForm.value.gender,
    };
    this.citizenService.createBasicCitizen(payload).subscribe({
      next: (res) => {
        this.isSaving.set(false);
        const parentBasicInfos: GetCitizensResponse = {
          id: res.data!,
          gender: payload.gender,
          firstName: payload.firstName,
          lastName: payload.lastName,
          birthDate: payload.birthDate,
          birthPlace: payload.birthPlace,
          nationality: payload.nationality,
        };
        this.dialogRef.close(parentBasicInfos);
      },
      error: () => {
        this.isSaving.set(false);
      },
    });
  }

  protected onCancel(): void {
    this.dialogRef.close();
  }
}
