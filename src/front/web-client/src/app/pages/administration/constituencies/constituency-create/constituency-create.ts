import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { ConstituencyApiService } from '@app/services/api/constituency.api.service';
import { ConstituencyModel } from '@app/services/nswag/api-nswag-client';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Constituency } from '../constituency/constituency';

@Component({
  selector: 'app-constituency-create',
  imports: [Constituency, CommonModule, TranslateModule],
  templateUrl: './constituency-create.html',
  styleUrl: './constituency-create.scss',
})
export class ConstituencyCreate {
  private readonly constituencyService = inject(ConstituencyApiService);
  private readonly translateService = inject(TranslateService);
  private readonly router = inject(Router);

  isSaving = signal(false);

  submittedForm(model: ConstituencyModel): void {
    this.isSaving.set(true);
    this.constituencyService
      .createConstituency(model, {
        successMessage: this.translateService.instant('constituency.successCreating'),
        errorMessage: this.translateService.instant('constituency.errorCreating'),
      })
      .subscribe({
        next: () => {
          this.isSaving.set(false);
          this.router.navigate(['/admin/constituencies']);
        },
        error: () => {
          this.isSaving.set(false);
        },
      });
  }

  goBack(): void {
    this.router.navigate(['/admin/constituencies']);
  }
}
