import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, input, output, signal } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { ConstituencyNode } from '@app/models/constituency.model';
import { Activity, allActivities } from '@app/pages/types/enumerations';
import { UsersApiService } from '@app/services/api/users.api.service';
import { AuthService } from '@app/services/auth/auth.service';
import { ConstituencyTreeHelperService } from '@app/services/constituency-tree-helper.service';
import { UserModel } from '@app/services/nswag/api-nswag-client';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
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
export class UserForm {
  form = input.required<UserFormFactory>();
  isSaving = input<boolean>(false);
  isUpdate = input<boolean>(false);
  constituencyId = input<number | undefined>(undefined);

  formSubmitted = output<UserModel>();
  goBack = output<void>();

  private readonly userService = inject(UsersApiService);
  private readonly authService = inject(AuthService);
  private readonly translateService = inject(TranslateService);
  private readonly store = inject(ConstituencyTreeHelperService);

  activityOptions: Activity[] = allActivities;

  // Signal pour gérer la visibilité du mot de passe
  hidePassword = signal(true);
  hideConfirmPassword = signal(true);

  nodes = this.store.nodesData;
  selectedNode = this.store.selectedNode;
  initialTreeSelectedId = computed(() => (!this.isUpdate() ? this.constituencyId() : undefined));

  // On ajoute un helper pour simplifier le template
  isRequester = computed(() => {
    const roles = this.form().controls.roles.value;
    return Array.isArray(roles) && roles.length === 1 && roles[0] === 'demandeur';
  });

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

  onNodeSelected(node: ConstituencyNode): void {
    this.store.setSelectedNode(node);

    // Mise à jour du formulaire avec la nouvelle circonscription sélectionnée
    this.form().patchValue({ constituencyId: node.id });
    this.form().controls.constituencyId.markAsDirty();
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
