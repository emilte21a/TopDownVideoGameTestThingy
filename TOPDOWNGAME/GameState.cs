public class GameState
{
    public Vector2 PlayerPosition { get; set; }
    public List<Vector2> EnemyPositions { get; set; } = new List<Vector2>();
    public int TimeElapsed { get; set; }
    public List<TileData> Tiles { get; set; } = new List<TileData>();
    public float CameraZoom { get; set; }
    public List<Entity> Entities { get; set; } = new List<Entity>();
    public List<InventoryItemData> Inventory {get; set;} = new List<InventoryItemData>();

}

public class TileData
{
    public Vector2 Position { get; set; }
    public int TileID { get; set; }
}

public class InventoryItemData
{
    public int ItemID {get; set;}
    public byte Amount {get; set;}
}

