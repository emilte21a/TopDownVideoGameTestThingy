
public abstract class Tile : GameObject
{
    public Rectangle rectangle;
    public Texture2D texture;
    public Vector2 position;
    public bool isSolid;
}

public class StoneTile : Tile
{
    static Texture2D stoneTexture;

    public StoneTile(Vector2 pos)
    {
        isSolid = true;
        tag = "Tile";
        position = pos;
        rectangle = new Rectangle(position.X, position.Y, 24, 24);

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

    public BackgroundTile(Vector2 pos)
    {
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