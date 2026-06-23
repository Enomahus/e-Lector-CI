import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DocumentApiService } from '@app/services/api/document.api.service';
import { RegistrationRequestApiService } from '@app/services/api/registration-request.api.service';
import {
  FileParameter,
  GetRegistrationRequestResponse,
  RegistrationRequestModel,
  RegistrationStatus,
  UpdateRegistrationRequestStatusCommand,
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
  private readonly documentService = inject(DocumentApiService);

  isSaving = signal(false);
  isLoading = signal(false);
  selectedStatus = signal<RegistrationStatus | null>(null);
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
        this.selectedStatus.set(response.data?.status!);
      },
      error: () => {
        this.isLoading.set(false);
        this.registrationRequest.set(undefined);
        this.constituencyId.set(undefined);
      },
    });
  }

  approveRequestTriggerred(event: {
    newStatus: RegistrationStatus;
    requestId: string;
    reason: string;
  }): void {
    this.isSaving.set(true);

    const command: UpdateRegistrationRequestStatusCommand = {
      reasonForRejection: event.reason,
      registrationRequestId: event.requestId,
      newStatus: event.newStatus,
      pollingStationId: 0,
    };

    this.registrationRequestService
      .updateRegistrationRequestStatus(event.requestId, command)
      .subscribe({
        next: () => {
          this.isSaving.set(false);
          this.goBack();
        },
        error: () => this.isSaving.set(false),
      });
  }

  viewDocument(documentId: string): void {
    this.documentService.downloadDocument(documentId).subscribe((blob) => {
      const url = window.URL.createObjectURL(blob.data);
      window.open(url, '_blank');
    });
  }

  async update(event: {
    registrationRequest: RegistrationRequestModel;
    certificateOfNationalityAttachments?: File;
    photoAttachments?: File;
  }): Promise<void> {
    this.isSaving.set(true);

    const { certificateOfNationalityAttachments, photoAttachments } =
      await this.handleAttachments(event);

    this.registrationRequestService
      .updateRegistrationRequest(
        this.registrationRequest()?.id!,
        event.registrationRequest,
        certificateOfNationalityAttachments,
        photoAttachments,
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
    photoAttachments?: File;
  }): Promise<{
    certificateOfNationalityAttachments?: FileParameter;
    cniAttachments?: FileParameter;
    photoAttachments?: FileParameter;
  }> {
    const certificateOfNationalityAttachments = await getFileParameter(
      event.certificateOfNationalityAttachments,
    );
    const photoAttachments = await getFileParameter(event.photoAttachments);

    return { certificateOfNationalityAttachments, photoAttachments };
  }

  goBack(): void {
    this.router.navigate(['registration-requests']);
  }
}
