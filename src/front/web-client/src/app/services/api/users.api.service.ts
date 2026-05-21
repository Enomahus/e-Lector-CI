import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import {
  CreateUserCommand,
  GetCurrentUserResponse,
  GetUsersQuery,
  RegisterUserCommand,
  Result,
  ResultOfListOfRoleModel,
  ResultOfPagedListOfGetUsersResponse,
  UpdateCurrentUserCommand,
  UpdateUserCommand,
  UserModel,
} from '../nswag/api-nswag-client';
import { ApiBaseService } from './api-base.service';
import { ApiToastOptions } from './models/api-toast-options';

@Injectable({
  providedIn: 'root',
})
export class UsersApiService extends ApiBaseService {
  udpateCurrentUser(
    command: UpdateCurrentUserCommand,
    options: ApiToastOptions = {},
  ): Observable<string> {
    return this.apiClient.updateCurrentUser(command).pipe(this.handleDataResult(options));
  }
  getCurrentUser(options: ApiToastOptions = {}): Observable<GetCurrentUserResponse> {
    return this.apiClient.getCurrentUser().pipe(this.handleDataResult(options));
  }

  getUser(id: string, options: ApiToastOptions = {}): Observable<UserModel> {
    return this.apiClient.getUser(id).pipe(this.handleDataResult(options));
  }

  getUsers(query: GetUsersQuery): Observable<ResultOfPagedListOfGetUsersResponse> {
    return this.apiClient.getUsers(query).pipe(this.handleResult());
  }

  createUser(command: CreateUserCommand, options: ApiToastOptions = {}): Observable<string> {
    return this.apiClient.createUser(command).pipe(this.handleDataResult(options));
  }

  udpateUser(
    userId: string,
    command: UpdateUserCommand,
    options: ApiToastOptions = {},
  ): Observable<string> {
    return this.apiClient.updateUser(userId, command).pipe(this.handleDataResult(options));
  }

  deleteUser(id: string, options: ApiToastOptions = {}): Observable<Result> {
    return this.apiClient.deleteUser(id).pipe(this.handleResult(options));
  }

  registerUser(command: RegisterUserCommand, options: ApiToastOptions = {}): Observable<string> {
    return this.apiClient.registerUser(command).pipe(this.handleDataResult(options));
  }

  getUserRoles(options: ApiToastOptions = {}): Observable<ResultOfListOfRoleModel> {
    return this.apiClient.getRoles().pipe(this.handleResult(options));
  }
}
