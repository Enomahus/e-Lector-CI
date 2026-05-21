import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RegistrationRequestApiService } from '@app/services/api/registration-request.api.service';
import { FileParameter, RegistrationRequestModel } from '@app/services/nswag/api-nswag-client';
import { getFileParameter } from '@app/shared/upload/file-upload-helper';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { RegistrationRequestUi } from '../registration-request-ui/registration-request-ui';

@Component({
  selector: 'app-create-registration-request-ui',
  imports: [TranslateModule, RegistrationRequestUi],
  templateUrl: './create-registration-request-ui.html',
  styleUrls: ['./create-registration-request-ui.scss'],
})
export class CreateRegistrationRequestUi {
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly registrationRequestService = inject(RegistrationRequestApiService);
  private readonly translateService = inject(TranslateService);
  isSaving = signal(false);
  constituencyId = computed(() => {
    const id = this.route.snapshot.queryParamMap.get('constituencyId');
    return id ? Number(id) : undefined;
  });

  async create(event: {
    registrationRequest: RegistrationRequestModel;
    certificateOfNationalityAttachments?: File;
    cniAttachments?: File;
    photoAttachments?: File;
  }): Promise<void> {
    this.isSaving.set(true);

    const { certificateOfNationalityAttachments, cniAttachments, photoAttachments } =
      await this.mapAttachments(event);

    this.registrationRequestService
      .createRegistrationRequest(
        event.registrationRequest,
        certificateOfNationalityAttachments ? [certificateOfNationalityAttachments] : undefined,
        cniAttachments ? [cniAttachments] : undefined,
        photoAttachments ? [photoAttachments] : undefined,
        {
          errorMessage: this.translateService.instant('registrationRequest.errorCreating'),
          successMessage: this.translateService.instant('registrationRequest.successCreating'),
        },
      )
      .subscribe({
        next: () => {
          this.isSaving.set(false);
          this.router.navigate(['registration-requests']);
        },
        error: () => this.isSaving.set(false),
      });
  }

  goBack(): void {
    this.router.navigate(['registration-requests']);
  }

  private async mapAttachments(event: {
    certificateOfNationalityAttachments?: File;
    cniAttachments?: File;
    photoAttachments?: File;
  }): Promise<{
    certificateOfNationalityAttachments?: FileParameter;
    cniAttachments?: FileParameter;
    photoAttachments?: FileParameter;
  }> {
    const certificateOfNationalityAttachments = await getFileParameter(
      event.certificateOfNationalityAttachments,
    );
    const cniAttachments = await getFileParameter(event.cniAttachments);
    const photoAttachments = await getFileParameter(event.photoAttachments);

    return { certificateOfNationalityAttachments, cniAttachments, photoAttachments };
  }
}
