using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Audit
{
    public enum AuditAction
    {
        RegistrationRequestValidated,
        RegistrationRequestCreated,
        RegistrationRequestUpdated,
        RegistrationRequestStatusUpdated,
        
        ConstituencyCreated,
        ConstituencyUpdated,
        ConstituencyDeleted,

        UserCreated,
        UserUpdated,
        UserDeleted,
        SuccesfullyAuthenticated
        
    }
}
