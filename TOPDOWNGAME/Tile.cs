public abstract class Tile : GameObject
{
    public Rectangle rectangle;
    public Texture2D texture;
    public Vector2 position;
    public bool isSolid;
    public Item itemToDrop;
    public byte tileID;
}

public class StoneTile : Tile
{
    static Texture2D stoneTexture;

    [JsonConstructor]
    public StoneTile(Vector2 pos)
    {
        tileID = 0;
        isSolid = true;
        tag = "Tile";
        position = pos;
        rectangle = new Rectangle(position.X, position.Y, 24, 24);

        itemToDrop = new StoneItem();

        if (stoneTexture.Id == 0)
        {
            stoneTexture = Raylib.LoadTexture("Images/StoneTexture.png");
        }
        texture = stoneTexture;
    }
}

public class BackgroundTile : Tile
{
    static Texture2D brickTexture;

    [JsonConstructor]
    public BackgroundTile(Vector2 pos)
    {
        tileID = 1;
        isSolid = false;
        tag = "Tile";
        position = pos;
        rectangle = new Rectangle(position.X, position.Y, 24, 24);

        if (brickTexture.Id == 0)
        {
            brickTexture = Raylib.LoadTexture("Images/BackGroundTexture.png");
        }
        texture = brickTexture;
    }
}

public class IronOre : Tile
{
    static Texture2D ironOreTexture;

    [JsonConstructor]
    public IronOre(Vector2 pos)
    {
        tileID = 2;
        isSolid = true;
        tag = "Tile";
        position = pos;
        rectangle = new Rectangle(position.X, position.Y, 24, 24);

        itemToDrop = new IronOreItem();

        if (ironOreTexture.Id == 0) ironOreTexture = Raylib.LoadTexture("Images/IronOreTexture.png");

        texture = ironOreTexture;
    }
}