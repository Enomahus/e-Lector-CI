import { CommonModule } from '@angular/common';
import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { ConstituencyApiService } from '@app/services/api/constituency.api.service';
import { ConstituencyModel } from '@app/services/nswag/api-nswag-client';
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
  constituency = signal<ConstituencyModel | undefined>(undefined);
  constituencyId = signal<number | undefined>(undefined);
  constituencyId$: Observable<number>;

  constructor() {
    this.constituencyId$ = this.route.paramMap.pipe(map((params) => Number(params.get('id'))));
  }

  ngOnInit(): void {
    this.constituencyId$
      .pipe(
        takeUntilDestroyed(this.destroyRef), // on détruit l'abonnement lorsque le composant est détruit
        tap((constituencyId) => {
          if (!constituencyId) {
            console.error('No constituency id provided from route');
            this.router.navigate(['/']);
          }
        }), // on redirige quand l'id est null ou undefined
        filter((constituencyId) => !!constituencyId),
        tap(() => this.isLoading.set(true)),
        switchMap((constituencyId) => this.constituencyService.getConstituency(constituencyId!)), // on récupère la circonscription
      )
      .subscribe({
        next: (res) => {
          this.constituency.set(res.data);
          //this.constituencyId.set(res?.data?.id);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
          this.router.navigate(['/']);
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
