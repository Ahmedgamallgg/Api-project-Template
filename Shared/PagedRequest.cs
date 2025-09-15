namespace Shared;

public sealed class PagedRequest
{
    public int PageIndex { get; init; } = 1;     // 1-based
    public int PageSize { get; init; } = 20;
    public string? SearchTerm { get; init; }
    public string? OrderBy { get; init; }     // e.g. "CreatedAt" or "Name"
    public bool OrderDesc { get; init; } = false;
}

