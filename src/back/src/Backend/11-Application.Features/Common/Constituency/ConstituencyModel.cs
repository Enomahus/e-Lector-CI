using Application.Common.Enums;

namespace Application.Features.Common.Constituency
{
    public class ConstituencyModel
    {
        public string? Code { get; set; }
        public string? Wording { get; set; }
        public LocationLevel Level { get; set; }
        public long? ParentId { get; set; }
        public bool IsActive { get; set; }
    }
}
