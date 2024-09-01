global using Raylib_cs;
global using System.Numerics;
global using System.Text.Json.Serialization;
using Raylib_CsLo;
using Raylib_CsLo.InternalHelpers;
using System.Text.Json;
using Camera2D = Raylib_cs.Camera2D;
using Color = Raylib_cs.Color;
using ConfigFlags = Raylib_cs.ConfigFlags;
using Font = Raylib_cs.Font;
using Image = Raylib_cs.Image;
using KeyboardKey = Raylib_cs.KeyboardKey;
using MouseButton = Raylib_cs.MouseButton;
using Raylib = Raylib_cs.Raylib;
using Rectangle = Raylib_cs.Rectangle;
using Sound = Raylib_cs.Sound;


public class Game
{
    string GAMENAME = "The dwell";

    public static int ScreenWidth = 1920;
    public static int ScreenHeight = 1080;
    public static Camera2D camera;
    Player player;
    List<GameSystem> gameSystems;
    WorldGeneration worldGeneration;
    List<GameObject> zSortList;
    public int timeElapsed = 0;

    public static Font customFont;

    float cameraZoom = 2.5f;

    public static List<Entity> entities = new List<Entity>();

    // Sound breakingSomething = Raylib.LoadSound("Audio/");

    List<GameState> gameSaves = new List<GameState>();

    enum CurrentScene
    {
        start, inGame
    }

    CurrentScene currentScene = CurrentScene.start;

    public Game()
    {
        Raylib.SetConfigFlags(ConfigFlags.FullscreenMode);
        Raylib.InitWindow(ScreenWidth, ScreenHeight, GAMENAME);
        Raylib.InitAudioDevice();
        Raylib.SetExitKey(KeyboardKey.Null);
        InitializeInstances();
        customFont = Raylib.LoadFontEx("Images/alagard.ttf", 120, null, 0);
        Console.WriteLine(Raylib.GetFontDefault());


        //gameSaves = LoadAllSaves();

        LoadAllGameSaves("Saves/saves.json");
    }

    private void InitializeInstances()
    {
        gameSystems = new();
        gameSystems.Add(new PhysicsSystem());
        gameSystems.Add(new CollisionSystem());


        camera = new()
        {
            Target = new Vector2(0, 0),
            Offset = new Vector2(ScreenWidth / 2, ScreenHeight / 2 + 60),
            Zoom = cameraZoom
        };

        player = new Player() { camera = camera };

        worldGeneration = new WorldGeneration();
        worldGeneration.GenerateWorld();


        entities.Add(player);

        zSortList = new();
        zSortList.Add(player);
        zSortList.Add(worldGeneration);

        for (int i = 0; i < player.abilitySystem.abilities.Count; i++)
            zSortList.Add(player.abilitySystem.abilities[i]);


        zSortList.Sort((a, b) => a.Z_layer.CompareTo(b.Z_layer));
    }



    public void Run()
    {
        while (!Raylib.WindowShouldClose())
        {
            Update();
            Draw();
        }
    }

    bool isGamePaused = false;

    bool shouldExitGame = false;

    bool exitToMainMenu = false;

    bool shouldStartNewGame = false;

    bool isLoadFilePageOpen = false;

    private void Update()
    {
        FullScreenOrNotIDK();
        if (currentScene == CurrentScene.start)
        {
            exitToMainMenu = false;
            if (shouldStartNewGame)
            {
                StartNewGame();
            }
        }

        if (currentScene == CurrentScene.inGame)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Escape) && !isGamePaused) isGamePaused = true;
            else if (Raylib.IsKeyPressed(KeyboardKey.Escape) && isGamePaused) isGamePaused = false;

            if (isGamePaused)
            {
                if (exitToMainMenu)
                {
                    currentScene = CurrentScene.start;
                    isLoadFilePageOpen = false;
                }
            }

            if (!isGamePaused)
            {
                timeElapsed += (int)Raylib.GetFrameTime();

                gameSystems[0].Update();
                player.Update();
                gameSystems[1].Update();
                camera.Target = Raymath.Vector2Lerp(camera.Target, player.position + new Vector2(12, 12), 8f * Raylib.GetFrameTime());

                cameraZoom += Raylib.GetMouseWheelMove() / 10f;
                cameraZoom = Raymath.Clamp(cameraZoom, 0.1f, 4);
                camera.Zoom = cameraZoom;

                Vector2 mousePos = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), camera);
                player.hoveringTile = worldGeneration.GetTile((int)mousePos.X / 24, (int)mousePos.Y / 24);

                if (player.hoveringTile != null && player.hoveringTile.isSolid && Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    player.inventory.AddToInventory(player.hoveringTile.itemToDrop);
                    worldGeneration.tileMap[(int)mousePos.X / 24, (int)mousePos.Y / 24] = null;
                    WorldGeneration.tilesInWorld.Remove(player.hoveringTile);
                }
                // else if ((player.hoveringTile == null || !player.hoveringTile.isSolid) && Raylib.IsMouseButtonPressed(MouseButton.Right) && player.inventory.currentActiveItem is IPlaceable placeable && player.inventory.currentActiveItem != null)
                // {
                //     Tile tempTile = placeable.TileToPlace(mousePos);
                //     worldGeneration.tileMap[(int)mousePos.X / 24, (int)mousePos.Y / 24] = tempTile;
                //     WorldGeneration.tilesInWorld.Add(tempTile);
                // }

                UpdateCameraShake();
            }
        }
    }





    private async void Draw()
    {
        Raylib.BeginDrawing();

        switch (currentScene)
        {

            case CurrentScene.start:

                Raylib.ClearBackground(Color.Black);
                Raylib.DrawTextEx(customFont, GAMENAME, new Vector2(100, 100), 80, 5, Color.White);
                if (isLoadFilePageOpen)
                {
                    for (int i = 0; i < gameSaves.Count; i++)
                    {
                        string buttonText = $"Save: {i + 1} - {gameSaves[i].SaveDate.ToString("g")}";
                        if (RayGui.GuiButton(new Raylib_CsLo.Rectangle(ScreenWidth / 2 - 100, ScreenHeight / 2 - 200 + i * 80, 200, 75), buttonText))
                        {
                            LoadGame(gameSaves[i].SaveName);
                            currentScene = CurrentScene.inGame;
                            isGamePaused = false;
                        }

                        if (RayGui.GuiButton(new Raylib_CsLo.Rectangle(ScreenWidth / 2 + 110, ScreenHeight / 2 - 200 + i * 80, 50, 75), "X"))
                            DeleteSave(gameSaves[i].SaveName);


                    }

                    if (RayGui.GuiButton(new Raylib_CsLo.Rectangle(100, ScreenHeight / 2 - 300, 200, 75), "Back") && isLoadFilePageOpen)
                        isLoadFilePageOpen = false;

                }
                else
                {
                    // Button to create a new save
                    if (RayGui.GuiButton(new Raylib_CsLo.Rectangle(100, ScreenHeight / 2 + 100, 200, 75), "New Save"))
                        StartNewGame();

                    if (RayGui.GuiButton(new Raylib_CsLo.Rectangle(100, ScreenHeight / 2 + 200, 200, 75), "Load world") && !isLoadFilePageOpen)
                        isLoadFilePageOpen = true;

                    shouldExitGame = RayGui.GuiButton(new Raylib_CsLo.Rectangle(100, ScreenHeight / 2 + 300, 200, 75), "Exit Game");
                }
                break;


            case CurrentScene.inGame:

                Raylib.ClearBackground(Color.White);
                Raylib.BeginMode2D(camera);

                foreach (var item in zSortList)
                {
                    item.Draw();
                }

                Raylib.DrawText("babbaaa", 0, 0, 20, Color.Orange);

                Vector2 playerScreenPos = Raylib.GetWorldToScreen2D(player.position, camera);
                Raylib.EndMode2D();

                player.abilitySystem.Draw();
                player.inventory.Draw();

                Raylib.DrawTextEx(customFont, $"{player.position}", new Vector2(0, 40), 20, 5, Color.Orange);
                Raylib.DrawTextEx(customFont, $"{InputManager.GetLastDirectionDelta()}", new Vector2(0, 80), 20, 5, Color.Orange);

                if (isGamePaused)
                {
                    if (RayGui.GuiButton(new Raylib_CsLo.Rectangle(Raylib.GetScreenWidth() / 2 - 150, Raylib.GetScreenHeight() / 2 - 50, 300, 75), "Exit To Main Menu"))
                    {
                        currentScene = CurrentScene.start;
                        isLoadFilePageOpen = false;
                    }
                }
                break;
        }
        Raylib.DrawFPS(0, 0);
        Raylib.EndDrawing();

        if (shouldExitGame)
        {
            SaveAllGameSaves("Saves/saves.JSON");
            Raylib.CloseWindow();
        }
    }

    static float shakeDuration;
    static float shakeMagnitude;

    public static void StartCameraShake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }

    void UpdateCameraShake()
    {
        if (shakeDuration > 0)
        {
            camera.Offset.X = ScreenWidth / 2 + (float)(Random.Shared.NextDouble() * 2 - 1) * shakeMagnitude;
            camera.Offset.Y = ScreenHeight / 2 + 60 + (float)(Random.Shared.NextDouble() * 2 - 1) * shakeMagnitude;
            shakeDuration -= Raylib.GetFrameTime();
            shakeMagnitude *= shakeDuration * 2f;
        }
        else
        {
            camera.Offset = new Vector2(ScreenWidth / 2, ScreenHeight / 2 + 60);
        }
    }

    private void FullScreenOrNotIDK()
    {
        ScreenWidth = Raylib.GetScreenWidth();
        ScreenHeight = Raylib.GetScreenHeight();

        if (Raylib.IsWindowFullscreen() && Raylib.IsKeyPressed(KeyboardKey.F11))
        {
            Raylib.SetWindowState(ConfigFlags.ResizableWindow);
        }
        else if (!Raylib.IsWindowFullscreen() && Raylib.IsKeyPressed(KeyboardKey.F11))
        {
            Raylib.SetWindowState(ConfigFlags.FullscreenMode);
        }
    }


    private void StartNewGame()
    {
        // Optionally, prompt the player for a save name
        string newSaveName = $"Save_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}";
        InitializeInstances();
        SaveGame(newSaveName);
        currentScene = CurrentScene.inGame;
        isGamePaused = false;
    }

    private GameState CaptureGameState()
    {
        var gameState = new GameState
        {
            PlayerPosition = player.position,
            Entities = new List<Entity>(entities),
            TimeElapsed = timeElapsed,
            CameraZoom = camera.Zoom,
            Tiles = new List<TileData>(),
            InventoryItemDatas = new List<InventoryItemData>()
        };

        // Capture tiles
        foreach (Tile tile in worldGeneration.tileMap)
        {
            if (tile != null)
            {
                gameState.Tiles.Add(new TileData
                {
                    Position = tile.position,
                    TileID = tile.tileID,
                });
            }
        }

        // Capture inventory items
        foreach (ItemSlot itemSlot in player.inventory.itemsInInventory)
        {
            if (itemSlot.item != null)
            {
                gameState.InventoryItemDatas.Add(new InventoryItemData
                {
                    ItemID = itemSlot.item.ID,
                    Amount = itemSlot.amount
                });
            }
        }

        // Debugging logs
        Console.WriteLine("Captured GameState:");
        Console.WriteLine(JsonSerializer.Serialize(gameState, new JsonSerializerOptions { WriteIndented = true }));

        return gameState;
    }

    private void ApplyGameState(GameState gameState)
    {
        player.position = gameState.PlayerPosition;
        timeElapsed = gameState.TimeElapsed;
        camera.Zoom = gameState.CameraZoom;
        entities.Clear();
        entities.Add(player);

        player.inventory = new Inventory();

        foreach (var itemData in gameState.InventoryItemDatas)
        {
            Item item = ItemRegistry.GetItemByID(itemData.ItemID);

            if (item != null)
            {
                for (int i = 0; i < itemData.Amount; i++)
                {
                    player.inventory.AddToInventory(item);
                }
            }
        }

        worldGeneration.tileMap = new Tile[worldGeneration.worldWidth, worldGeneration.worldHeight];
        WorldGeneration.tilesInWorld.Clear();

        foreach (var tileData in gameState.Tiles)
        {
            Tile? tile = tileData.TileID switch
            {
                0 => new StoneTile(tileData.Position),
                1 => new BackgroundTile(tileData.Position),
                2 => new IronOre(tileData.Position),
                _ => null
            };
            if (tile != null)
            {
                worldGeneration.tileMap[(int)tile.position.X / 24, (int)tile.position.Y / 24] = tile;
                WorldGeneration.tilesInWorld.Add(tile);
            }
        }
    }

    private void SaveGame(string saveName)
    {
        // Ensure the directory exists
        string directoryPath = "bin/Saves";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        GameState gameState = CaptureGameState();
        gameState.SaveName = saveName;
        gameState.SaveDate = DateTime.Now;

        string jsonString = JsonSerializer.Serialize(gameState, new JsonSerializerOptions { WriteIndented = true });
        string filePath = Path.Combine(directoryPath, $"{saveName}.json");

        try
        {
            File.WriteAllText(filePath, jsonString);
            Console.WriteLine($"Game saved successfully to {filePath}.");
            // Optionally, update the in-memory gameSaves list
            // var existingSave = gameSaves.FirstOrDefault(s => s.SaveName == saveName);
            // if (existingSave != null)
            // {
            //     gameSaves.Remove(existingSave);
            // }
            gameSaves.Add(gameState);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save game: {ex.Message}");
        }
    }

    private void LoadGame(string saveName)
    {
        string filePath = Path.Combine("Saves", $"{saveName}.json");
        try
        {
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                GameState? gameState = JsonSerializer.Deserialize<GameState>(jsonString);

                if (gameState == null)
                {
                    Console.WriteLine("Failed to deserialize game state.");
                    return;
                }

                ApplyGameState(gameState);
                Console.WriteLine($"Game loaded successfully from {filePath}.");
            }
            else
            {
                Console.WriteLine("Save file not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load game: {ex.Message}");
        }
    }

    private List<GameState> LoadAllSaves()
    {
        List<GameState> saves = new List<GameState>();
        string directoryPath = "Saves";

        if (Directory.Exists(directoryPath))
        {
            var saveFiles = Directory.GetFiles(directoryPath, "*.json");
            foreach (var file in saveFiles)
            {
                try
                {
                    string jsonString = File.ReadAllText(file);
                    GameState? gameState = JsonSerializer.Deserialize<GameState>(jsonString);
                    if (gameState != null)
                    {
                        saves.Add(gameState);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load save from {file}: {ex.Message}");
                }
            }
        }

        return saves;
    }

    private void DeleteSave(string saveName)
    {
        string filePath = Path.Combine("Saves", $"{saveName}.json");
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                var save = gameSaves.FirstOrDefault(s => s.SaveName == saveName);
                if (save != null)
                {
                    gameSaves.Remove(save);
                }
                Console.WriteLine($"Save '{saveName}' deleted successfully.");
            }
            else
            {
                Console.WriteLine("Save file not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete save: {ex.Message}");
        }
    }

    private void SaveAllGameSaves(string filePath)
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(gameSaves, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
            Console.WriteLine("All game saves saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save all game saves: {ex.Message}");
        }
    }

    private void LoadAllGameSaves(string filePath)
    {
        if (File.Exists(filePath))
        {
            try
            {
                string jsonString = File.ReadAllText(filePath);
                gameSaves = JsonSerializer.Deserialize<List<GameState>>(jsonString) ?? new List<GameState>();
                Console.WriteLine("All game saves loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load game saves: {ex.Message}");
                gameSaves = new List<GameState>();
            }
        }
        else
        {
            Console.WriteLine("No game saves file found, starting fresh.");
            gameSaves = new List<GameState>();
        }
    }
}