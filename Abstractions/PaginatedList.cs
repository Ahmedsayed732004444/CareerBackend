namespace Career_Path.Abstractions;

public class PaginatedList<T>(List<T> items, int pageNumber, int count, int pageSize)
{
    public List<T> Items { get; private set; } = items;
    public int PageNumber { get; private set; } = pageNumber;
    public int TotalPages { get; private set; } = (int)Math.Ceiling(count / (double)pageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;


    // PaginatedList.cs
    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await source.CountAsync(cancellationToken);
            var items = await source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedList<T>(items, pageNumber, count, pageSize);
        }
        catch (OperationCanceledException)
        {
            // طبيعي - TanStack Query كنسل الـ request القديم
            return new PaginatedList<T>([], pageNumber, 0, pageSize);
        }
    }
}