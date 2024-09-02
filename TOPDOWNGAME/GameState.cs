public class GameState
{
    public string SaveName { get; set; } = "Default Save";
    public DateTime SaveDate { get; set; } = DateTime.Now;
    public Vector2 PlayerPosition { get; set; }
    public int TimeElapsed { get; set; }
    public List<TileData> Tiles { get; set; } 
    public float CameraZoom { get; set; }
    public List<Entity> Entities { get; set; } 
    public List<InventoryItemData> InventoryItemDatas { get; set; } 

    public GameState()
    {
        Tiles = new List<TileData>();
        InventoryItemDatas = new List<InventoryItemData>();
    }

}

public class TileData
{
    public Vector2 Position { get; set; }
    public int TileID { get; set; }
}

public class InventoryItemData
{
    public int ItemID { get; set; }
    public byte Amount { get; set; }
}

