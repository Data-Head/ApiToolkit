namespace ApiToolkit.ViewModels;

public class PaginatedData<T>
{
    public int Page { get; set; } = 0;
    public int PageCount { get; set; } = 0;
    public int RecordsPerPage { get; set; } = 0;
    public int? TotalRecords { get; set; } = null;
    public IEnumerable<T> Data { get; set; } = null!;
}
