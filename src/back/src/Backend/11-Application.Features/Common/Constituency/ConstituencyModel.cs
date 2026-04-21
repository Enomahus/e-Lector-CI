using Application.Common.Enums;
using Infrastructure.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Common.Constituency
{
    public class ConstituencyModel
    {
        public string? Code { get; set; } 
        public string? Wording { get; set; } 
        public LocationLevel Level { get; set; }
        public long? ParentId { get; set; }
        public bool IsActive { get; set; }

        public static ConstituencyModel From(ConstituencyDao dao, DateTimeOffset dateNow)
        {
            return new ConstituencyModel
            {
                Code = dao.Code,
                Wording = dao.Wording,
                Level = dao.Level,
                ParentId = dao.ParentId,
                IsActive = dao.DisabledDate is null || dao.DisabledDate > dateNow
            };
        }

        public ConstituencyDao ToDao()
        {
            return new ConstituencyDao()
            {
                Code = Code,
                Wording = Wording,
                Level = Level,
                ParentId = ParentId,                
            };
        }
    }
}
