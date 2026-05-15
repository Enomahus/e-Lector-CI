using Application.Features.Common.Constituency;
using Infrastructure.Persistence.Entities;

namespace Application.Features.Constituency.GetConstituency
{
    public class GetConstituencyResponse : ConstituencyModel
    {
        public required long Id { get; set; }
        public string? Code { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public static GetConstituencyResponse FromDao(ConstituencyDao dao, DateTimeOffset dateNow)
        {
            return new GetConstituencyResponse()
            {
                Id = dao.Id,
                Code = dao.Code,
                Wording = dao.Wording,
                Level = dao.Level,
                ParentId = dao.ParentId,
                CreatedAt = dao.CreatedAt,
                IsActive = dao.DisabledDate is null || dao.DisabledDate > dateNow,
            };
        }
    }
}
