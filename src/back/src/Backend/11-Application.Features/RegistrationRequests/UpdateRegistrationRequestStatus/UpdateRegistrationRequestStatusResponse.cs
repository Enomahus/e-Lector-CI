using Application.Common.Enums;
using Application.Features.Common.Citizen;
using Application.Features.RegistrationRequests.GetRegistrationRequest;
using Application.Features.Users.Common;
using Infrastructure.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.RegistrationRequests.UpdateRegistrationRequestStatus
{
    public class UpdateRegistrationRequestStatusResponse
    { 
        public Guid Id { get; set; }
        public RegistrationStatus Status { get; set; }
        public string? ReasonForRejection  { get; set; }
        public static UpdateRegistrationRequestStatusResponse FromDao(RegistrationRequestDao dao)
        {
            return new UpdateRegistrationRequestStatusResponse()
            {
                Id = dao.Id,
                Status = dao.Status,
                ReasonForRejection = dao.ReasonForRejection,
            };
        }
    }
}
