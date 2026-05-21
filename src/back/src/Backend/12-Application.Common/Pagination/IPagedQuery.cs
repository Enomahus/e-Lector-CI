namespace Application.Common.Pagination;

public interface IPagedQuery
{
    string? Sort { get; }
    string? Order { get; }
    int? PageIndex { get; }
    int PageSize { get; }
    string? Search { get; }
}
