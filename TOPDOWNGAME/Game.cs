global using Raylib_cs;
global using System.Numerics;
using Raylib_CsLo;
using Camera2D = Raylib_cs.Camera2D;
using Color = Raylib_cs.Color;
using ConfigFlags = Raylib_cs.ConfigFlags;
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

    float cameraZoom = 2.5f;

    public static List<Entity> entities = new List<Entity>();

    // Sound breakingSomething = Raylib.LoadSound("Audio/");

    public Game()
    {
        Raylib.SetConfigFlags(ConfigFlags.FullscreenMode);
        Raylib.InitWindow(ScreenWidth, ScreenHeight, "Game");
        Raylib.InitAudioDevice();
        Raylib.SetExitKey(KeyboardKey.Null);
        InitializeInstances();
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
        if (Raylib.IsKeyPressed(KeyboardKey.Escape) && !pause) pause = true;
        else if (Raylib.IsKeyPressed(KeyboardKey.Escape) && pause) pause = false;

        FullScreenOrNotIDK();

        if (!pause)
        {
            System.Console.WriteLine(Raylib.GetKeyPressed());
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
                worldGeneration.tileMap[(int)mousePos.X / 24, (int)mousePos.Y / 24] = null;
                WorldGeneration.tilesInWorld.Remove(player.hoveringTile);
            }

            UpdateCameraShake();
        }
    }

    bool exit;

    private void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Gray);
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
        Raylib.DrawText($"{player.speedMultiplier}", 0, 120, 20, Color.Red);
        Raylib.DrawText($"{player.inventory.currentItemIndex}", 0, 160, 20, Color.Red);


        // Raylib.BeginBlendMode(Raylib_cs.BlendMode.Additive);
        // Raylib.DrawRectangle(0, 0, ScreenWidth, ScreenHeight, Color.Black);
        // Raylib.DrawCircleGradient((int)playerScreenPos.X, (int)playerScreenPos.Y, 300, Color.Blank, Color.Black);
        // Raylib.EndBlendMode();

        Raylib.DrawFPS(0, 0);
        Raylib.DrawText($"{player.position}", 0, 40, 20, Color.Orange);
        Raylib.DrawText($"{InputManager.GetLastDirectionDelta()}", 0, 80, 20, Color.Orange);


        if (pause)
            exit = RayGui.GuiButton(new Raylib_CsLo.Rectangle(Raylib.GetScreenWidth() / 2 - 150, Raylib.GetScreenHeight() / 2 - 75, 300, 150), "Exit");

        Raylib.EndDrawing();
        if (exit) Raylib.CloseWindow();
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
}