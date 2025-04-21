namespace Kromi.Application.Data.Models.GenericQueries
{
    public record PaginationQuery
    {
        private const int _maxPageSize = 1000;
        public int PageNumber { get; set; }

        private int _pageSize = 10;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value > _maxPageSize ? _maxPageSize : value;
            }
        }
    }
}
