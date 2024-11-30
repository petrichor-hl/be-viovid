namespace VioVid.Core.Common;

public class PaginationResponse<T>
{
    public int TotalCount { get; set; }

    public IEnumerable<T> Items { get; set; }

    public int PageIndex { get; set; }

    public int PageSize { get; set; }
}