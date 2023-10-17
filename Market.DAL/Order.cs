namespace Market.DAL;

public class Order
{
    public Guid Id { get; set; }
    public List<ItemEntry> ItemEntries { get; set; }
}

public class ItemEntry
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public Item Item { get; set; }
    public int Count { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; }
}

public class Item
{
    public Guid Id { get; set; }
}