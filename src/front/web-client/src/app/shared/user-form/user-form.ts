import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, input, OnInit, output, signal } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { ConstituencyNode } from '@app/models/constituency.model';
import { UsersApiService } from '@app/services/api/users.api.service';
import { AuthService } from '@app/services/auth/auth.service';
import { ConstituencyTreeHelperService } from '@app/services/constituency-tree-helper.service';
import { RoleModel, UserModel } from '@app/services/nswag/api-nswag-client';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { tap } from 'rxjs';
import { ConstituencyTree } from '../constituency-tree/constituency-tree';
import { Loader } from '../loader/loader';
import { PhoneInput } from '../phone-input/phone-input';
import { StickyButtonsContainer } from '../sticky-buttons-container/sticky-buttons-container';
import { UserFormFactory } from './user-form-factory';

@Component({
  selector: 'app-user-form',
  imports: [
    ReactiveFormsModule,
    CommonModule,
    TranslateModule,
    PhoneInput,
    ConstituencyTree,
    Loader,
    StickyButtonsContainer,
  ],
  templateUrl: './user-form.html',
  styleUrls: ['./user-form.scss'],
})
export class UserForm implements OnInit {
  form = input.required<UserFormFactory>();
  isSaving = input<boolean>(false);
  isEditMode = input<boolean>(false);
  constituencyId = input<number | undefined>(undefined);

  formSubmitted = output<UserModel>();
  goBack = output<void>();

  private readonly userService = inject(UsersApiService);
  private readonly authService = inject(AuthService);
  private readonly translateService = inject(TranslateService);
  private readonly store = inject(ConstituencyTreeHelperService);

  availableRoles = ['requester', 'agent', 'admin'];
  roles!: RoleModel[];

  // Signal pour gérer la visibilité du mot de passe
  hidePassword = signal(true);
  hideConfirmPassword = signal(true);

  nodes = this.store.nodesData;
  selectedNode = this.store.selectedNode;
  initialTreeSelectedId = computed(() => (!this.isEditMode() ? this.constituencyId() : undefined));

  // On ajoute un helper pour simplifier le template
  isRequester = computed(() => {
    const roles = this.form().controls.roles.value;
    return Array.isArray(roles) && roles.length === 1 && roles[0] === 'requester';
  });

  requiredEmployeeNumber(): boolean {
    const roles = this.form().controls.roles.value;
    return roles.some((r) => r === 'admin' || r === 'agent');
  }

  constructor() {
    effect(() => {
      const nodes = this.nodes();
      const id = this.constituencyId();

      if (nodes.length > 0 && id) {
        this.store.expandNodePath(id);
        this.store.setSelectedNode(id);
      }
    });
  }

  ngOnInit(): void {
    this.userService
      .getUserRoles()
      .pipe(tap((roles) => (this.roles = roles.data ?? [])))
      .subscribe();
  }

  onNodeSelected(node: ConstituencyNode): void {
    this.store.setSelectedNode(node);

    // Mise à jour du formulaire avec la nouvelle circonscription sélectionnée
    this.form().patchValue({ constituencyId: node.id });
    this.form().controls.constituencyId.markAsDirty();
  }

  toggleRole(role: string): void {
    const currentRoles = this.form().controls.roles?.value ?? [];
    const newRoles = currentRoles.includes(role)
      ? currentRoles.filter((r) => r !== role)
      : [...currentRoles, role];
    this.form().controls.roles.setValue(newRoles);
    this.form().updateValueAndValidity();
  }

  hasRole(role: string): boolean {
    const roles = this.form().controls.roles.value || [];
    return roles.includes(role);
  }

  onSubmit(): void {
    this.form().markAllAsTouched();

    if (this.form().invalid) return;

    const formValue: UserModel = this.form().getRawValue();
    this.formSubmitted.emit(formValue);
  }

  togglePassword() {
    this.hidePassword.update((v) => !v);
  }

  toggleConfirmPassword() {
    this.hideConfirmPassword.update((v) => !v);
  }
}
