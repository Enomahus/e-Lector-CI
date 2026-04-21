import { AsyncPipe } from '@angular/common';
import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { Breadcrumbs } from '@app/models/breadcrumb.model';
import { BreadcrumbService } from '@app/services/breadcrumb.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-breadcrumb',
  imports: [AsyncPipe, RouterLink],
  templateUrl: './breadcrumb.html',
  styleUrl: './breadcrumb.scss',
})
export class Breadcrumb implements OnInit {
  breadcrumbs$?: Observable<Breadcrumbs[]>;

  breadcrumbService = inject(BreadcrumbService);
  destroyRef = inject(DestroyRef);

  ngOnInit(): void {
    this.breadcrumbs$ = this.breadcrumbService.breadcrumbs$.pipe(
      takeUntilDestroyed(this.destroyRef),
    );
  }
}
