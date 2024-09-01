public static class ItemRegistry
{
    private static Dictionary<int, Item> itemRegistry = new Dictionary<int, Item>();

    // Static constructor to initialize the item registry
    static ItemRegistry()
    {
        // Register all item types here with their unique IDs
        itemRegistry[0] = new StoneItem();
        itemRegistry[1] = new IronOreItem();
        
    }

    public static Item GetItemByID(int id)
    {
        if (itemRegistry.TryGetValue(id, out Item item))
        {
            // Return a new instance of the item
            // If the items are mutable, you may need to clone them instead
            return item;
        }

        // If the item ID doesn't exist, return null or throw an exception
        return null;
    }
}