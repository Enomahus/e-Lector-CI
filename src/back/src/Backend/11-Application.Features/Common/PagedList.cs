namespace Application.Features.Common
{
    public record PagedList<T>(List<T> Items, int TotalCount);
}
