# Introduction

An audit module for MediatR commands

## Dependencies

- .NET 10
- MediatR

## Usage

To use this project, you first need to implement the [IAuditService](./Interfaces/IAuditService.cs) interface in your Application layer.
Your IAuditService implementation will act as the bridge between your app and the audit module, to let it fetch informations about the action triggered (current user, time), but also to save the logs to your store of choice.

Then, from your Application services configuration, call [AddPceaCoreNetAuditTrailServices](./ConfigureServices.cs), passing the type of your IAuditService implementation. This will configure the dependency injection for the IAuditService, and also add the audit behaviour to the MediatR pipeline.

With the dependencies configured, all that is left to do is decorate the commands you wish to see audited with the attribute [AuditParameters](./Attributes/AuditParametersAttribute.cs).

You will have to specify a Category and an Action name as string.
Any string will do, but it is recommended to extract them from an enum, to keep track of all keys in the application.

Optionally, you can enhance your logs by specifying the subject targeted by your actions.
To do so, you can have your commands return a result that implements the [IAuditableResult](./Interfaces/IAuditableResult.cs). The audit behaviour will detect that informations were attached to your result, and add them to the log.
