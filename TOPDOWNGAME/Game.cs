global using Raylib_cs;
global using System.Numerics;
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

    enum CurrentScene
    {
        start, inGame
    }

    CurrentScene currentScene;

    public Game()
    {
        Raylib.SetConfigFlags(ConfigFlags.FullscreenMode);
        Raylib.InitWindow(ScreenWidth, ScreenHeight, "Game");
        Raylib.InitAudioDevice();
        Raylib.SetExitKey(KeyboardKey.Null);
        InitializeInstances();
        currentScene = CurrentScene.start;
        customFont = Raylib.LoadFontEx("Images/alagard.ttf", 120, null, 0);
        System.Console.WriteLine(Raylib.GetFontDefault());
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
        {
            zSortList.Add(player.abilitySystem.abilities[i]);
        }

        zSortList.Sort((a, b) => a.Z_layer.CompareTo(b.Z_layer));


    }

    bool pause = false;

    bool save = false;

    bool load = false;

    bool exitGame = false;

    bool exitToMainMenu = false;

    bool startNewGame = false;

    public void Run()
    {
        while (!Raylib.WindowShouldClose())
        {
            Update();
            Draw();
        }
    }



    private void Update()
    {
        FullScreenOrNotIDK();
        if (currentScene == CurrentScene.start)
        {
            exitToMainMenu = false;
            if (startNewGame)
            {
                StartNewGame();
                currentScene = CurrentScene.inGame;
                pause = false;
            }

            if (load)
            {
                LoadGame("Saves/savegame.JSON");
                currentScene = CurrentScene.inGame;
                pause = false;
            }

        }

        if (currentScene == CurrentScene.inGame)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Escape) && !pause) pause = true;
            else if (Raylib.IsKeyPressed(KeyboardKey.Escape) && pause) pause = false;

            if (pause)
                if (exitToMainMenu) currentScene = CurrentScene.start;

            if (!pause)
            {
                timeElapsed += (int)Raylib.GetFrameTime();
                if (save)
                {
                    SaveGame("Saves/savegame.JSON");
                }

                gameSystems[0].Update();
                player.Update();
                gameSystems[1].Update();
                camera.Target = Raymath.Vector2Lerp(camera.Target, player.position + new Vector2(12, 12), 8f * Raylib.GetFrameTime());

                cameraZoom += Raylib.GetMouseWheelMove() / 10f;
                cameraZoom = Raymath.Clamp(cameraZoom, 0, 4);
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





    private void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.White);

        if (currentScene == CurrentScene.start)
        {
            startNewGame = RayGui.GuiButton(new Raylib_CsLo.Rectangle(ScreenWidth / 2 - 100, ScreenHeight / 2 - 200, 200, 75), "New Save");
            load = RayGui.GuiButton(new Raylib_CsLo.Rectangle(ScreenWidth / 2 - 100, ScreenHeight / 2 - 100, 200, 75), "Load Save");
            exitGame = RayGui.GuiButton(new Raylib_CsLo.Rectangle(ScreenWidth / 2 - 100, ScreenHeight / 2, 200, 75), "Exit Game");
        }

        if (currentScene == CurrentScene.inGame)
        {
            Raylib.BeginMode2D(camera);

            foreach (var item in zSortList)
            {
                item.Draw();
            }
            // worldGeneration.Draw();
            // player.Draw();

            Raylib.DrawText("babbaaa", 0, 0, 20, Color.Orange);


            Vector2 playerScreenPos = Raylib.GetWorldToScreen2D(player.position, camera);
            Raylib.EndMode2D();

            player.abilitySystem.Draw();
            player.inventory.Draw();

            // Raylib.BeginBlendMode(Raylib_cs.BlendMode.Additive);
            // Raylib.DrawRectangle(0, 0, ScreenWidth, ScreenHeight, Color.Black);
            // Raylib.DrawCircleGradient((int)playerScreenPos.X, (int)playerScreenPos.Y, 300, Color.Blank, Color.Black);
            // Raylib.EndBlendMode();

            Raylib.DrawFPS(0, 0);
            Raylib.DrawTextEx(customFont, $"{player.position}", new Vector2(0, 40), 20, 5, Color.Orange);
            Raylib.DrawTextEx(customFont, $"{InputManager.GetLastDirectionDelta()}", new Vector2(0, 80), 20, 5, Color.Orange);


            if (pause)
            {
                save = RayGui.GuiButton(new Raylib_CsLo.Rectangle(Raylib.GetScreenWidth() / 2 - 150, Raylib.GetScreenHeight() / 2 - 125, 300, 150), "Save");
                exitToMainMenu = RayGui.GuiButton(new Raylib_CsLo.Rectangle(Raylib.GetScreenWidth() / 2 - 150, Raylib.GetScreenHeight() / 2 + 75, 300, 150), "Exit To Main Menu");
            }
        }

        Raylib.EndDrawing();
        if (exitGame) Raylib.CloseWindow();
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
        InitializeInstances();
    }

    private GameState CaptureGameState()
    {
        var gameState = new GameState
        {
            PlayerPosition = player.position,
            Entities = entities,
            TimeElapsed = timeElapsed,
            CameraZoom = camera.Zoom,
            // Add any other necessary properties
        };

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

        foreach (var itemSlot in player.inventory.itemsInInventory)
        {
            if (itemSlot.item != null)
            {
                gameState.Inventory.Add(new InventoryItemData
                {
                    ItemID = itemSlot.item.ID,
                    Amount = itemSlot.amount
                });
            }
        }


        // Logging the gameState for debugging
        Console.WriteLine("Captured GameState: ");
        Console.WriteLine(JsonSerializer.Serialize(gameState, new JsonSerializerOptions { WriteIndented = true }));
        // Add any other game state capturing here

        return gameState;
    }

    private void ApplyGameState(GameState gameState)
    {
        player.position = gameState.PlayerPosition;
        timeElapsed = gameState.TimeElapsed;
        camera.Zoom = gameState.CameraZoom;

        player.inventory = new Inventory();

        foreach (var itemData in gameState.Inventory)
        {
            // Assuming you have a way to retrieve an item by its ID
            Item item = ItemRegistry.GetItemByID(itemData.ItemID);

            if (item != null)
            {
                for (int i = 0; i < itemData.Amount; i++)
                {
                    player.inventory.AddToInventory(item);
                }
            }
            else
            {
                Console.WriteLine($"Item with ID {itemData.ItemID} not found.");
            }
        }

        worldGeneration.tileMap = new Tile[worldGeneration.worldWidth, worldGeneration.worldHeight]; // or however your tileMap is initialized
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
            else
            {
                Console.WriteLine($"Tile with ID {tileData.TileID} not found.");
            }
        }

        // Apply any other loaded state to the game
    }

    private void SaveGame(string filePath)
    {
        // Ensure the directory exists
        string directoryPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        GameState gameState = CaptureGameState();
        string jsonString = JsonSerializer.Serialize(gameState, new JsonSerializerOptions { WriteIndented = true });

        try
        {
            File.WriteAllText(filePath, jsonString);
            Console.WriteLine("Game saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save game: {ex.Message}");
        }
    }

    private void LoadGame(string filePath)
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

            // Logging the loaded GameState for debugging
            Console.WriteLine("Loaded GameState: ");
            Console.WriteLine(JsonSerializer.Serialize(gameState, new JsonSerializerOptions { WriteIndented = true }));

            ApplyGameState(gameState);
            Console.WriteLine("Game loaded successfully.");
        }
        else
        {
            Console.WriteLine("Save file not found.");
        }
    }
}