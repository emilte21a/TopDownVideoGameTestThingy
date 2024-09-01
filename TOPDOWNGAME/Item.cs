public interface IPlaceable
{
    public Tile TileToPlace(Vector2 pos);
}

public abstract class Item
{
    public bool craftable;
    public bool usable;
    public Texture2D icon;
    public byte ID;
}

public class StoneItem : Item, IPlaceable
{
    private static Texture2D stoneTexture;
    public StoneItem()
    {
        ID = 0;
        craftable = false;
        usable = false;
        if (stoneTexture.Id == 0)
        {
            stoneTexture = Raylib.LoadTexture("Images/StoneTexture.png");
        }
        icon = stoneTexture;
    }

    public Tile TileToPlace(Vector2 pos)
    {
        return new StoneTile(pos);
    }
}

public class IronOreItem : Item
{
    private static Texture2D ironOreTexture;
    public IronOreItem()
    {
        ID = 1;
        craftable = false;
        usable = false;
        if (ironOreTexture.Id == 0)
        {
            ironOreTexture = Raylib.LoadTexture("Images/IronOreTexture.png");
        }
        icon = ironOreTexture;
    }
    public Tile TileToPlace(Vector2 pos)
    {
        return new IronOre(pos);
    }
}