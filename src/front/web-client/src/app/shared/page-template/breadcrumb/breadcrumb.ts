import { AsyncPipe } from '@angular/common';
import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { Breadcrumbs } from '../../../models/breadcrumb.model';
import { BreadcrumbService } from '../../../services/breadcrumb.service';

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
