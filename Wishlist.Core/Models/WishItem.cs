namespace Wishlist.Core;

public class WishItem
{
    private string Name { get; } = string.Empty;
    private string Description { get; } = string.Empty;
    private decimal Cost { get; }
    private string ItemUrl { get; } = string.Empty;
    public WishItem(string name, string description, decimal cost, string itemUrl)
    {
        Name = name;
        Description = description;
        Cost = cost;
        ItemUrl = itemUrl;
    }
}
