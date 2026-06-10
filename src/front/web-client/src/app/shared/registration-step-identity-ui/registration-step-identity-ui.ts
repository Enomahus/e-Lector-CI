import { Component, effect, input } from '@angular/core';
import { ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { IdentityForm } from '@app/pages/registration-request-home/registration-wizard-ui/registration-wizard-form';
import {
  allGenders,
  allMaritalStatus,
  allPersonTitle,
  allRegistrationRequestType,
} from '@app/pages/types/enumerations';
import { TranslateModule } from '@ngx-translate/core';
import { InputDatepickerUi } from '../input-datepicker-ui/input-datepicker-ui';

@Component({
  selector: 'app-registration-step-identity-ui',
  imports: [
    ReactiveFormsModule,
    TranslateModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    InputDatepickerUi,
  ],
  templateUrl: './registration-step-identity-ui.html',
  styleUrl: './registration-step-identity-ui.scss',
})
export class RegistrationStepIdentityUi {
  stepForm = input.required<IdentityForm>();

  allRegistrationRequestType = allRegistrationRequestType;
  allMaritalStatus = allMaritalStatus;
  allGenders = allGenders;
  allPersonTitle = allPersonTitle;

  constructor() {
    effect(() => {
      const isMarried = this.stepForm().value.maritalStatus === 'married';
      const marriedName = this.stepForm().controls.marriedName;

      if (isMarried) {
        marriedName.setValidators([Validators.required]);
      } else {
        marriedName.clearValidators();
        marriedName.setValue(undefined);
      }
      marriedName.updateValueAndValidity();
    });
  }
}
