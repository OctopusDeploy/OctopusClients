namespace Octopus.Client.Model.RateLimiting;

public class ListRateLimitingPoliciesResponse(
    string itemType,
    int totalResults,
    int itemsPerPage,
    int numberOfPages,
    int lastPageNumber,
    RateLimitingPolicyResource[] items)
{
    public string ItemType { get; } = itemType;
    public int TotalResults { get; } = totalResults;
    public int ItemsPerPage { get; } = itemsPerPage;
    public int NumberOfPages { get; } = numberOfPages;
    public int LastPageNumber { get; } = lastPageNumber;
    public RateLimitingPolicyResource[] Items { get; } = items;
}
