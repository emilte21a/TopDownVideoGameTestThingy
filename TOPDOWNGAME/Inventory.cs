

public class Inventory
{
    byte maxItems = 5;
    byte iconSizeUI = 48;
    public ItemSlot[] itemsInInventory;
    public int currentItemIndex = 0;
    public Item? currentActiveItem;

    public Inventory()
    {
        itemsInInventory = new ItemSlot[maxItems];
        for (int i = 0; i < itemsInInventory.Length; i++)
        {
            itemsInInventory[i] = new ItemSlot(null)
            {
                keyIndex = (KeyboardKey)(i + 49)
            };
        }
    }

    public void AddToInventory(Item item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item), "Item cannot be null.");
        }

        for (int i = 0; i < itemsInInventory.Length; i++)
        {
            if (itemsInInventory[i].item != null && itemsInInventory[i].item.ID.Equals(item.ID))
            {
                itemsInInventory[i].amount++;
                return;
            }
        }

        int emptySlot = FindFirstEmptySlot();
        if (emptySlot != -1)
        {
            itemsInInventory[emptySlot].item = item;
            itemsInInventory[emptySlot].amount = 1;
        }
    }

    public void Update()
    {

        UpdateCurrentItemIndex();

        for (int i = 0; i < itemsInInventory.Length; i++)
        {
            if (i == currentItemIndex)
                itemsInInventory[currentItemIndex].color = Color.Yellow;

            else
                itemsInInventory[i].color = Color.White;
        }

        currentActiveItem = itemsInInventory[currentItemIndex].item;
    }

    public void Draw()
    {
        float startX = (Game.ScreenWidth - (iconSizeUI + 12) * itemsInInventory.Length) / 2f;
        float fixedY = Game.ScreenHeight - 200;

        for (int i = 0; i < itemsInInventory.Length; i++)
        {
            Vector2 abilityPositionUI = new Vector2(startX + i * (iconSizeUI + 12), fixedY);

            Raylib.DrawRectangle((int)abilityPositionUI.X - 2, (int)abilityPositionUI.Y - 2, iconSizeUI + 4, iconSizeUI + 4, Color.Black);
            Raylib.DrawRectangleLines((int)abilityPositionUI.X - 2, (int)abilityPositionUI.Y - 2, iconSizeUI + 4, iconSizeUI + 4, itemsInInventory[i].color);

            if (itemsInInventory[i].item != null)
            {
                Raylib.DrawTexture(itemsInInventory[i].item.icon, (int)abilityPositionUI.X, (int)abilityPositionUI.Y, Color.White);
                Raylib.DrawTextEx(Game.customFont, $"{itemsInInventory[i].amount}", new Vector2((int)abilityPositionUI.X + 24, (int)abilityPositionUI.Y + 24), 18, 2, Color.White);
            }
        }
    }

    public void UpdateCurrentItemIndex()
    {
        for (int i = 0; i < itemsInInventory.Length; i++)
        {
            if (Raylib.IsKeyPressed(itemsInInventory[i].keyIndex))
            {
                currentItemIndex = i;
                break;
            }
        }
    }

    private int FindFirstEmptySlot()
    {
        for (int i = 0; i < itemsInInventory.Length; i++)
        {
            if (itemsInInventory[i].item == null)
            {
                return i;
            }
            continue;
        }
        return -1;
    }

}

public class ItemSlot
{
    public Item item;
    public byte amount;
    public Color color;
    public KeyboardKey keyIndex;

    [JsonConstructor]
    public ItemSlot(Item _item)
    {
        item = _item;
        amount = (item != null) ? (byte)1 : (byte)0;
        color = Color.White;
    }
}