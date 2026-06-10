import { DatePipe } from '@angular/common';
import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FiliationForm } from '@app/pages/registration-request-home/registration-wizard-ui/registration-wizard-form';
import { CitizenApiService } from '@app/services/api/citizen.api.service';
import { Gender, GetCitizensResponse, MaritalStatus } from '@app/services/nswag/api-nswag-client';
import { TranslateModule } from '@ngx-translate/core';
import { ParentModalUi } from '../parent-modal-ui/parent-modal-ui';

@Component({
  selector: 'app-registration-step-filiation-ui',
  imports: [ReactiveFormsModule, TranslateModule, MatFormFieldModule, MatInputModule],
  templateUrl: './registration-step-filiation-ui.html',
  styleUrl: './registration-step-filiation-ui.scss',
  providers: [DatePipe],
})
export class RegistrationStepFiliationUi implements OnInit {
  stepForm = input.required<FiliationForm>();
  situation = input.required<MaritalStatus>();

  private readonly datePipe = inject(DatePipe);
  private readonly citizenService = inject(CitizenApiService);
  private readonly dialog = inject(MatDialog);

  parents = signal<GetCitizensResponse[]>([]);
  isLoading = signal<boolean>(false);
  isEditMode = signal<boolean>(false);

  // Signaux pour gérer l'état d'affichage strict des inputs
  protected fatherInputValue = signal<string>('');
  protected motherInputValue = signal<string>('');
  // Computed properties pour filtrer les datalists
  protected filteredFatherOptions = computed(() => this.filterParents(this.fatherInputValue()));
  protected filteredMotherOptions = computed(() => this.filterParents(this.motherInputValue()));

  ngOnInit(): void {
    // const idParam = this.route.snapshot.params['id'];
    // if (idParam) {
    //   const id = Number(idParam);
    //   if (!isNaN(id)) {
    //     this.registrationRequestId.set(id);
    //     this.isEditMode.set(true);
    //   }
    // }
    this.loadCitizens();
  }

  private loadCitizens(): void {
    this.citizenService.getCitizens().subscribe((response) => {
      this.parents.set(response.data ?? []);
      this.syncInputValues();
    });
  }

  private filterParents(query: string): GetCitizensResponse[] {
    const q = query.toLowerCase().trim();
    if (!q) return this.parents();
    return this.parents().filter(
      (p) => p.firstName?.toLowerCase().includes(q) || p.lastName?.toLowerCase().includes(q),
    );
  }

  protected getParentDisplayName(parent: GetCitizensResponse): string {
    const date = parent.birthDate ? this.datePipe.transform(parent.birthDate, 'dd/MM/yyyy') : 'N/A';
    return `${parent.firstName} ${parent.lastName?.toUpperCase()} (${date}) ${parent.birthPlace}`.trim();
  }

  private syncInputValues(): void {
    const fId = this.stepForm().controls.fatherId.value;
    if (fId) {
      const parent = this.parents().find((p) => p.id === fId);
      if (parent) this.fatherInputValue.set(this.getParentDisplayName(parent));
    }

    const mId = this.stepForm().controls.motherId.value;
    if (mId) {
      const parent = this.parents().find((p) => p.id === mId);
      if (parent) this.motherInputValue.set(this.getParentDisplayName(parent));
    }
  }

  protected onParentSearchChange(event: Event, parentType: 'father' | 'mother'): void {
    const inputVal = (event.target as HTMLInputElement).value;
    const isFather = parentType === 'father';

    if (isFather) this.fatherInputValue.set(inputVal);
    else this.motherInputValue.set(inputVal);

    const matchedParent = this.parents().find((p) => this.getParentDisplayName(p) === inputVal);

    const control = this.stepForm().get(`${parentType}Id`);
    if (control) {
      if (matchedParent) {
        control.setValue(matchedParent.id);
      } else {
        control.setValue(undefined);
      }
      control.markAsDirty();
    }
  }

  protected openAddParentModal(gender: Gender): void {
    const dialogRef = this.dialog.open(ParentModalUi, {
      width: '470px',
      data: gender,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((newParent: GetCitizensResponse | undefined) => {
      if (newParent && newParent.id) {
        this.parents.update((currentParents) => [...currentParents, newParent]);

        const isFather = gender.toLowerCase() === 'm';
        const controlName = isFather ? 'fatherId' : 'motherId';
        const control = this.stepForm().get(controlName);

        if (control) {
          control.setValue(newParent.id);
          control.markAsDirty();
          control.updateValueAndValidity();
        }

        if (isFather) {
          this.fatherInputValue.set(this.getParentDisplayName(newParent));
        } else {
          this.motherInputValue.set(this.getParentDisplayName(newParent));
        }
      }
    });
  }
}
