import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Breadcrumbs } from '@app/models/breadcrumb.model';
import { ConstituencyApiService } from '@app/services/api/constituency.api.service';
import { BreadcrumbService } from '@app/services/breadcrumb.service';
import { ConstituencyModel } from '@app/services/nswag/api-nswag-client';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Constituency } from '../constituency/constituency';
import { ConstituencyForm, createConstituencyForm } from '../constituency/constituency-form';

@Component({
  selector: 'app-constituency-create',
  imports: [Constituency, CommonModule, TranslateModule],
  templateUrl: './constituency-create.html',
  styleUrl: './constituency-create.scss',
})
export class ConstituencyCreate implements OnInit {
  private readonly constituencyService = inject(ConstituencyApiService);
  private readonly translateService = inject(TranslateService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly breadcrumbService = inject(BreadcrumbService);

  isSaving = signal(false);
  form = signal<ConstituencyForm>(createConstituencyForm());
  constituencyId = signal<number | undefined>(undefined);

  ngOnInit(): void {
    const constituencyId = this.route.snapshot.queryParamMap.get('constituencyId');
    if (constituencyId) {
      this.constituencyId.set(Number(constituencyId));
    }
    this.setBreadcrumb();
  }

  private setBreadcrumb(): void {
    let breadcrumbs: Breadcrumbs[] = [];
    breadcrumbs = [
      {
        label: this.translateService.instant('breadcrumb.constituencies'),
        url: `/admin/constituencies`,
      },
      {
        label: this.translateService.instant('breadcrumb.constituencyCreate'),
      },
    ];
    this.breadcrumbService.setBreadcrumbs(breadcrumbs);
  }

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
