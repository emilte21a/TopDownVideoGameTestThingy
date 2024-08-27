public abstract class Item
{
    public bool craftable;
    public bool usable;
    public Texture2D icon;
}

public class StoneItem : Item
{
    private static Texture2D stoneTexture;
    public StoneItem()
    {
        craftable = false;
        usable = false;
        if (stoneTexture.Id == 0)
        {
            stoneTexture = Raylib.LoadTexture("Images/StoneTexture.png");
        }
        icon = stoneTexture;
    }
}