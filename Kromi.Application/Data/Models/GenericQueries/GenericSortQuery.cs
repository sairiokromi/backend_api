using Kromi.Domain.Extensions;

namespace Kromi.Application.Data.Models.GenericQueries
{
    public record GenericSortQuery
    {
        public string? Field { get; set; } = string.Empty;
        public SortOrder Order { get; set; } = SortOrder.ASC;
    }
}
