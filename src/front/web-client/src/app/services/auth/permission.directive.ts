import {
  Directive,
  inject,
  Input,
  OnDestroy,
  signal,
  TemplateRef,
  ViewContainerRef,
} from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { AppPermission } from '../nswag/api-nswag-client';
import { AuthService } from './auth.service';

@Directive({
  selector: '[appHasPermission]',
})
export class PermissionDirective implements OnDestroy {
  private readonly authService = inject(AuthService);
  private readonly viewContainer = inject(ViewContainerRef);
  private readonly templateRef = inject(TemplateRef<unknown>);

  private readonly destroy$ = new Subject<void>();
  private isViewCreated = signal(false);

  @Input() set appHasPermission(permission: AppPermission | AppPermission[] | undefined) {
    this.destroy$.next();

    if (!permission || (Array.isArray(permission) && permission.length === 0)) {
      this.updateView(true);
      return;
    }

    const requiredPermissions = Array.isArray(permission) ? permission : [permission];

    this.authService
      .getPermissions()
      .pipe(takeUntil(this.destroy$))
      .subscribe((userPerms) => {
        const hasPermission = userPerms.some((up) => requiredPermissions.includes(up));
        this.updateView(hasPermission);
      });
  }

  private updateView(hasPermission: boolean): void {
    if (hasPermission && !this.isViewCreated()) {
      this.viewContainer.createEmbeddedView(this.templateRef);
      this.isViewCreated.set(true);
    } else if (!hasPermission && this.isViewCreated()) {
      this.viewContainer.clear();
      this.isViewCreated.set(false);
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
