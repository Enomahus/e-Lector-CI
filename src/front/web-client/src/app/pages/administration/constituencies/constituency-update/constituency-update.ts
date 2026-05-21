import { CommonModule } from '@angular/common';
import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { ConstituencyApiService } from '@app/services/api/constituency.api.service';
import { ConstituencyModel, GetConstituencyResponse } from '@app/services/nswag/api-nswag-client';
import { Loader } from '@app/shared/loader/loader';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { filter, map, Observable, switchMap, tap } from 'rxjs';
import { Constituency } from '../constituency/constituency';

@Component({
  selector: 'app-constituency-update',
  imports: [TranslateModule, CommonModule, Loader, Constituency],
  templateUrl: './constituency-update.html',
  styleUrl: './constituency-update.scss',
})
export class ConstituencyUpdate implements OnInit {
  private readonly constituencyService = inject(ConstituencyApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly translateService = inject(TranslateService);
  private readonly destroyRef = inject(DestroyRef);

  isLoading = signal(false);
  isSaving = signal(false);
  constituency = signal<GetConstituencyResponse | undefined>(undefined);
  constituencyId = signal<number | undefined>(undefined);
  constituencyId$: Observable<number>;

  constructor() {
    this.constituencyId$ = this.route.params.pipe(map((param) => parseInt(param['id'])));
  }

  ngOnInit(): void {
    this.constituencyId$
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        tap((id) => {
          if (!id) {
            console.error('No constituency id provided in route');
            this.router.navigate(['admin/constituencies']);
          }
        }),
        filter((id) => !!id),
        tap(() => this.isLoading.set(true)),
        switchMap((id) =>
          this.constituencyService.getConstituency(id, {
            errorMessage: this.translateService.instant('constituency.errorLoading'),
          }),
        ),
      )
      .subscribe({
        next: (constituency) => {
          this.constituency.set(constituency.data!);
          this.constituencyId.set(constituency.data!.id);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
          this.router.navigate(['admin/constituencies']);
        },
      });
  }

  submitedForm(model: ConstituencyModel): void {
    if (!this.constituencyId()) return;

    this.isSaving.set(true);

    this.constituencyService
      .updateConstituency(this.constituencyId()!, model, {
        successMessage: this.translateService.instant('constituency.successUpdating'),
        errorMessage: this.translateService.instant('constituency.errorUpdating'),
      })
      .subscribe({
        next: () => {
          this.isSaving.set(false);
          this.navigateToConstituencies();
        },
        error: () => {
          this.isSaving.set(false);
        },
      });
  }

  navigateToConstituencies(): void {
    this.router.navigate(['admin/constituencies']);
  }
}
