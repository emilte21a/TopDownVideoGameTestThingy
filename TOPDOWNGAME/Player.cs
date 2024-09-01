

public class Player : Entity
{
    public Camera2D camera { get; init; }
    PhysicsBody physicsBody;
    Collider collider;
    Animator animator;
    public Inventory inventory;

    public AbilitySystem abilitySystem;

    public Tile hoveringTile;

    Texture2D idleSide = Raylib.LoadTexture("Images/IdleSide.png");
    Texture2D idleUp = Raylib.LoadTexture("Images/IdleUp.png");
    Texture2D IdleDown = Raylib.LoadTexture("Images/IdleDown.png");
    Texture2D runningAnimationSide = Raylib.LoadTexture("Images/RunningSideAnimation.png");
    Texture2D runningAnimationUp = Raylib.LoadTexture("Images/RunningUpAnimation.png");
    Texture2D runningAnimationDown = Raylib.LoadTexture("Images/RunningDownAnimation.png");

    public float stamina = 100;
    float staminaTimer = 2;
    bool isRunning = false;

    public int speedMultiplier = 1;

    public Player()
    {
        tag = "Player";
        Z_layer = 3;
        components = new();
        physicsBody = AddComponent<PhysicsBody>();
        collider = AddComponent<Collider>();
        animator = AddComponent<Animator>();

        abilitySystem = new AbilitySystem();
        abilitySystem.AddAbility(new Dash());
        abilitySystem.AddAbility(new ShockWave());

        inventory = new Inventory();
        inventory.AddToInventory(new StoneItem());

        physicsBody.UseGravity = PhysicsBody.Gravity.enabled;
        physicsBody.gravity = Vector2.Zero;

        rectangle = new Rectangle(0, 0, 24, 24);
        position = new Vector2(rectangle.X, rectangle.Y);
        collider.boxCollider = rectangle;
    }

    public override void Update()
    {
        for (int i = 0; i < abilitySystem.abilities.Count; i++)
        {
            if (abilitySystem.abilities[i].GetType() == typeof(ShockWave))
                abilitySystem.abilities[i].position = position - new Vector2(24, 24);

        }

        if ((physicsBody.velocity.X != 0 || physicsBody.velocity.Y != 0) && Raylib.IsKeyDown(KeyboardKey.LeftShift) && stamina > 0)
        {
            isRunning = true;
        }
        else
            isRunning = false;

        if (isRunning)
        {
            speedMultiplier = 2;
            stamina -= 20 * Raylib.GetFrameTime();
            staminaTimer = 2;
        }

        if (!isRunning && stamina < 100)
        {
            speedMultiplier = 1;
            staminaTimer -= Raylib.GetFrameTime();

            if (staminaTimer <= 0)
                stamina += 20 * Raylib.GetFrameTime();
        }

        stamina = Raymath.Clamp(stamina, 0, 100);

        position = new Vector2(rectangle.X, rectangle.Y);
        collider.boxCollider.X = rectangle.X;
        collider.boxCollider.Y = rectangle.Y;
        MovePlayer(physicsBody, 0.9f * speedMultiplier);
        abilitySystem.Update();
        inventory.Update();
    }


    Color staminaColor;
    public override void Draw()
    {
        if (physicsBody.velocity.X == 0 && physicsBody.velocity.Y == 0)
        {
            if (InputManager.GetLastDirectionDelta() == 1 || InputManager.GetLastDirectionDelta() == 2)
            {
                Raylib.DrawTextureRec(idleSide, new Rectangle(0, 0, 24 * lastDirection.X, 28), new Vector2(position.X, position.Y - 4), Color.White);
            }
            else if (InputManager.GetLastDirectionDelta() == 3)
            {
                Raylib.DrawTextureRec(idleUp, new Rectangle(0, 0, 24, 28), new Vector2(position.X, position.Y - 4), Color.White);
            }
            else if (InputManager.GetLastDirectionDelta() == 4)
            {
                Raylib.DrawTextureRec(IdleDown, new Rectangle(0, 0, 24, 28), new Vector2(position.X, position.Y - 4), Color.White);
            }
            // animator.PlayAnimation(idleAnimation, (int)lastDirection.X, 2, new Vector2(position.X, position.Y - 4), 5);
        }

        else if (physicsBody.velocity.X != 0) animator.PlayAnimation(runningAnimationSide, (int)lastDirection.X, 4, new Vector2(position.X - 4, position.Y - 4), 15 * speedMultiplier);
        else if (physicsBody.velocity.Y > 0) animator.PlayAnimation(runningAnimationDown, (int)lastDirection.X, 4, new Vector2(position.X - 4, position.Y - 4), 15 * speedMultiplier);
        else if (physicsBody.velocity.Y < 0) animator.PlayAnimation(runningAnimationUp, (int)lastDirection.X, 4, new Vector2(position.X - 4, position.Y - 4), 15 * speedMultiplier);

        if (stamina != 100)
            staminaColor = Color.White;

        else
            staminaColor = Color.Orange;


        Raylib.DrawRectangle((int)position.X - 12, (int)position.Y - 10, 50, 2, new Color(255, 255, 255, 120));
        Raylib.DrawRectangle((int)position.X - 12, (int)position.Y - 10, (int)stamina / 2, 2, staminaColor);

        Raylib.DrawTextEx(Game.customFont, $"{hoveringTile}", new Vector2((int)position.X, (int)position.Y - 20), 5, 2, Color.SkyBlue);
        Raylib.DrawTextEx(Game.customFont, $"{inventory.currentActiveItem}", new Vector2((int)position.X, (int)position.Y - 50), 5, 2, Color.SkyBlue);
    }

    public void MovePlayer(PhysicsBody physicsBody, float speed)
    {
        physicsBody.velocity.X = Raymath.Clamp(InputManager.GetAxisX() * speed, -5f, 5f);
        physicsBody.velocity.Y = Raymath.Clamp(InputManager.GetAxisY() * speed, -5f, 5f);
    }
}