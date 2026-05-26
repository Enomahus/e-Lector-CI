import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RegistrationRequestApiService } from '@app/services/api/registration-request.api.service';
import {
  FileParameter,
  GetRegistrationRequestResponse,
  RegistrationRequestModel,
} from '@app/services/nswag/api-nswag-client';
import { Loader } from '@app/shared/loader/loader';
import { getFileParameter } from '@app/shared/upload/file-upload-helper';
import { TranslateModule } from '@ngx-translate/core';
import { RegistrationRequestUi } from '../registration-request-ui/registration-request-ui';

@Component({
  selector: 'app-update-registration-request-ui',
  imports: [TranslateModule, RegistrationRequestUi, Loader],
  templateUrl: './update-registration-request-ui.html',
  styleUrls: ['./update-registration-request-ui.scss'],
})
export class UpdateRegistrationRequestUi implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly registrationRequestService = inject(RegistrationRequestApiService);
  isSaving = signal(false);
  isLoading = signal(false);
  constituencyId = signal<number | undefined>(undefined);
  registrationRequest = signal<GetRegistrationRequestResponse | undefined>(undefined);
  registrationRequestId = computed(() => {
    const id = this.route.snapshot.paramMap.get('id');
    return id ?? undefined;
  });

  ngOnInit(): void {
    this.loadRegistrationRequest();
  }

  private loadRegistrationRequest(): void {
    const id = this.registrationRequestId();
    if (!id) return;

    this.isLoading.set(true);
    this.registrationRequestService.getRegistrationRequest(id).subscribe({
      next: (response) => {
        this.isLoading.set(false);
        this.registrationRequest.set(response.data);
        this.constituencyId.set(response.data?.constituencyId);
      },
      error: () => {
        this.isLoading.set(false);
        this.registrationRequest.set(undefined);
        this.constituencyId.set(undefined);
      },
    });
  }

  async update(event: {
    registrationRequest: RegistrationRequestModel;
    certificateOfNationalityAttachments?: File;
    cniAttachments?: File;
    photoAttachments?: File;
  }): Promise<void> {
    this.isSaving.set(true);

    const { certificateOfNationalityAttachments, cniAttachments, photoAttachments } =
      await this.handleAttachments(event);

    this.registrationRequestService
      .updateRegistrationRequest(
        this.registrationRequest()?.id!,
        event.registrationRequest,
        certificateOfNationalityAttachments ? certificateOfNationalityAttachments : undefined,
        cniAttachments ? cniAttachments : undefined,
        photoAttachments ? photoAttachments : undefined,
        {},
      )
      .subscribe({
        next: () => {
          this.isSaving.set(false);
          this.goBack();
        },
        error: () => {
          this.isSaving.set(false);
        },
      });
  }

  private async handleAttachments(event: {
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

  goBack(): void {
    this.router.navigate(['registration-requests']);
  }
}
