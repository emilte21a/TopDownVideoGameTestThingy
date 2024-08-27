public class WorldGeneration : GameObject
{

    public static List<Tile> tilesInWorld;
    public static List<Tile> tilesThatShouldRender;

    public int worldWidth = 100, worldHeight = 100;

    private byte threshold = 126;

    private int seed;

    public Tile[,] tileMap;

    Texture2D tileTexture = Raylib.LoadTexture("Images/TileSheet.png");
    int numTilesInColumn = 4;
    int numTilesInRow = 4;

    public WorldGeneration()
    {
        Z_layer = 1;
        tilesInWorld = new List<Tile>();
        tilesThatShouldRender = new List<Tile>();
        seed = Random.Shared.Next(-10000, 10000);
        tileMap = new Tile[worldWidth, worldHeight];
    }

    public void GenerateWorld()
    {
        tilesInWorld.Clear();
        Image perlinNoise = Raylib.GenImagePerlinNoise(1000, 1000, seed, seed, 40f);

        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                if (Raylib.GetImageColor(perlinNoise, x, y).R > threshold)
                {
                    StoneTile stoneTile = new StoneTile(new Vector2(x * 24, y * 24));
                    PlaceTile(stoneTile);
                    tileMap[x, y] = stoneTile;
                }

                else
                {
                    BackgroundTile backgroundTile = new BackgroundTile(new Vector2(x * 24, y * 24));
                    PlaceTile(backgroundTile);
                    tileMap[x, y] = backgroundTile;
                }

            }
        }
        Raylib.UnloadImage(perlinNoise);
    }

    public override void Draw()
    {
        for (int i = 0; i < tilesThatShouldRender.Count; i++)
        {
            Tile tile = tilesThatShouldRender[i];
            if (tile.GetType() == typeof(StoneTile))
            {
                // Convert pixel position to tile coordinates
                int tileXCoord = (int)(tile.position.X / 24);
                int tileYCoord = (int)(tile.position.Y / 24);

                // Calculate the correct tile index
                int tileIndex = GetTileIndex(tileXCoord, tileYCoord);
                int tileX = tileIndices[tileIndex] % numTilesInRow;
                int tileY = tileIndices[tileIndex] / numTilesInRow;

                Rectangle sourceRec = new Rectangle(tileX * 24, tileY * 24, 24, 24);
                Rectangle destRec = new Rectangle((int)tile.position.X, (int)tile.position.Y, 24, 24);

                Raylib.DrawTexturePro(tileTexture, sourceRec, destRec, Vector2.Zero, 0, Color.White);
            }
            else
            {
                Raylib.DrawTexture(tile.texture, (int)tile.position.X, (int)tile.position.Y, Color.White);
            }
        }
    }


    public Tile GetTile(int x, int y)
    {
        if (x < 0 || x >= worldWidth || y < 0 || y >= worldHeight) return null;
        return tileMap[x, y];
    }

    int GetTileIndex(int x, int y)
    {
        bool top = GetTile(x, y - 1)?.GetType() == typeof(StoneTile);
        bool right = GetTile(x + 1, y)?.GetType() == typeof(StoneTile);
        bool bottom = GetTile(x, y + 1)?.GetType() == typeof(StoneTile);
        bool left = GetTile(x - 1, y)?.GetType() == typeof(StoneTile);

        int index = 0;
        if (top) index |= 1;         // 0001
        if (right) index |= 2;       // 0010
        if (bottom) index |= 4;      // 0100
        if (left) index |= 8;        // 1000

        return index;
    }

    int[] tileIndices =
    {
        0, // 0000
        1, // 0001
        2, // 0010
        3, // 0011
        4, // 0100
        5, // 0101
        6, // 0110
        7, // 0111
        8, // 1000
        9, // 1001
        10, // 1010
        11, // 1011
        12, // 1100
        13, // 1101
        14, // 1110
        15 // 1111
    };



    public void PlaceTile(Tile tile)
    {
        tilesInWorld.Add(tile);
    }
}