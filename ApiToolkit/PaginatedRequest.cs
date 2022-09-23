using MediatR;

namespace ApiToolkit;

public class PaginatedRequest<T> : IRequest<T>
{
    public int Page { get; set; } = 1;
    public int CountPerPage { get; set; } = 20;
}