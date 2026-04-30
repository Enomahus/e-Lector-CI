import { CommonModule } from '@angular/common';
import { Component, inject, input, OnInit, output, signal } from '@angular/core';
import {
  FormControl,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Breadcrumbs } from '@app/models/breadcrumb.model';
import { ConstituencyApiService } from '@app/services/api/constituency.api.service';
import { BreadcrumbService } from '@app/services/breadcrumb.service';
import { ConstituencyModel, LocationLevel } from '@app/services/nswag/api-nswag-client';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-constituency',
  imports: [TranslateModule, CommonModule, ReactiveFormsModule],
  templateUrl: './constituency.html',
  styleUrl: './constituency.scss',
})
export class Constituency implements OnInit {
  private readonly constituecyService = inject(ConstituencyApiService);
  private readonly translateService = inject(TranslateService);
  private readonly breadcrumbService = inject(BreadcrumbService);
  private readonly route = inject(ActivatedRoute);

  constituency = input<ConstituencyModel | undefined>(undefined);
  constituencyId = input<number | undefined>(undefined);
  saveConstituency = output<ConstituencyModel>();
  goBack = output<void>();
  isSaving = input.required<boolean>();
  isToCreate = input.required<boolean>();

  private fg = inject(NonNullableFormBuilder);

  // Liste des niveaux pour le select
  levels: LocationLevel[] = [
    'region',
    'department',
    'subPrefecture',
    'municipality',
    'votingLocation',
  ];

  constituencyForm = this.fg.group({
    code: new FormControl<string>('', [Validators.required]),
    wording: new FormControl<string>('', [Validators.required]),
    level: new FormControl<LocationLevel>('votingLocation', { nonNullable: true }),
    parentId: new FormControl<number | undefined>(undefined, [Validators.required]),
    isActive: new FormControl<boolean>(true, [Validators.required]),
  });

  formData = signal<ConstituencyModel | null>(null);

  ngOnInit(): void {
    this.setBreadcrumbs(this.constituency());

    if (!this.isToCreate() && this.constituency()) {
      this.constituencyForm.patchValue(this.constituency()!);
    }
  }

  setBreadcrumbs(constituency: ConstituencyModel | undefined): void {
    let breadcrumbs: Breadcrumbs[] = [];
    breadcrumbs = [
      {
        label: this.translateService.instant('constituencies.title'),
        url: '/admin/constituencies',
      },
      {
        label: this.isToCreate()
          ? this.translateService.instant('constituency.newConstituencyTitle')
          : (constituency?.wording ?? ''),
      },
    ];
    this.breadcrumbService.setBreadcrumbs(breadcrumbs);
  }

  save(): void {
    if (this.constituencyForm.invalid) {
      this.constituencyForm.markAsTouched();
      return;
    }
    const rawValue = this.constituencyForm.getRawValue();
    this.formData.set(rawValue as ConstituencyModel);
    this.saveConstituency.emit(this.formData()!);
    this.setBreadcrumbs(this.formData()!);
  }
}
