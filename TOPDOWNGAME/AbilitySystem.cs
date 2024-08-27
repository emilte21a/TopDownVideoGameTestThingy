public class AbilitySystem
{
    byte iconSizeUI = 24;
    public List<Ability> abilities;
    public AbilitySystem()
    {
        abilities = new List<Ability>();
    }

    public void AddAbility(Ability ability)
    {
        abilities.Add(ability);
    }

    public void Update()
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (i == 0) abilities[i].keyToActivate = KeyboardKey.F;
            else if (i == 1) abilities[i].keyToActivate = KeyboardKey.T;
            else if (i == 2) abilities[i].keyToActivate = KeyboardKey.C;


            if (Raylib.IsKeyPressed(abilities[i].keyToActivate) && !abilities[i].isActive)
            {
                abilities[i].isActive = true;
                abilities[i].timer = 0;

                abilities[i].OnUse();
            }



            if (abilities[i].isActive)
            {
                if (abilities[i].timer < abilities[i].duration)
                    abilities[i].timer += Raylib.GetFrameTime();

                else
                    abilities[i].isActive = false;

            }


        }
    }

    public void Draw()
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            int opacity;
            if (abilities[i].isActive) opacity = 128;

            else opacity = 255;


            Vector2 abilityPositionUI = new Vector2(Game.ScreenWidth / 2 - iconSizeUI / 2 - abilities.Count * iconSizeUI / 2 + i * (iconSizeUI + 12), Game.ScreenHeight - 300);
            Raylib.DrawRectangle((int)abilityPositionUI.X - 2, (int)abilityPositionUI.Y - 2, iconSizeUI + 4, iconSizeUI + 4, Color.Black);
            Raylib.DrawRectangleLines((int)abilityPositionUI.X - 2, (int)abilityPositionUI.Y - 2, iconSizeUI + 4, iconSizeUI + 4, Color.White);
            Raylib.DrawTexture(abilities[i].icon, (int)abilityPositionUI.X, (int)abilityPositionUI.Y, new Color(255, opacity, opacity, opacity));
            Raylib.DrawText($"{abilities[i].keyToActivate}", (int)abilityPositionUI.X, (int)abilityPositionUI.Y - 12, 10, Color.White);
        }
    }
}

public abstract class Ability : GameObject
{
    public float duration;
    public bool dealsDamage;
    public Texture2D icon;
    public bool isActive;
    public float timer = 0;
    public Vector2 position;
    public KeyboardKey keyToActivate;
    public Animator animator;

    public virtual void OnUse() { }
}

public class Dash : Ability
{

    public Dash()
    {
        Z_layer = 2;
        components = new();
        animator = AddComponent<Animator>();
        isActive = false;
        duration = 0.5f;
        dealsDamage = false;
        icon = Raylib.LoadTexture("Images/DashAbility.png");
    }
    public override void OnUse()
    {
        base.OnUse();
    }
}

public class ShockWave : Ability
{
    static Texture2D shockWave;
    public ShockWave()
    {
        Z_layer = 2;
        components = new();
        animator = AddComponent<Animator>();
        isActive = false;
        duration = 0.5f;
        dealsDamage = true;
        icon = Raylib.LoadTexture("Images/ShockWaveAbility.png");
        shockWave = Raylib.LoadTexture("Images/ShockWaveBlast.png");
    }

    public override void OnUse()
    {
        animator.frameStart = 0;
        Game.StartCameraShake(duration, 10);
    }
    public override void Draw()
    {
        if (isActive)
            animator.PlayAnimation(shockWave, 1, 5, position, 22);
    }
}