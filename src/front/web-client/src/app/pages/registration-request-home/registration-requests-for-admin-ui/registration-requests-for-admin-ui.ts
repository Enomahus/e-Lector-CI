import { Component } from '@angular/core';
import { GetRegistrationRequestsResponse } from '@app/services/nswag/api-nswag-client';
import { AbstractRegistrationRequestsUI } from '../generic-registration-requests/abstract-registration-requests-ui';

@Component({
  selector: 'app-registration-requests-for-admin-ui',
  imports: [],
  templateUrl: '../generic-registration-requests/generic-registration-requests.html',
  styleUrls: ['../generic-registration-requests/generic-registration-requests.scss'],
})
export class RegistrationRequestsForAdminUi extends AbstractRegistrationRequestsUI<GetRegistrationRequestsResponse> {
  protected override titleKey = 'registrationRequests.titleForAdmin';
  protected override routePrefix = 'registration-requests-for-admin';
}
