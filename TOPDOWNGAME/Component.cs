public abstract class Component { }

public class Collider : Component
{
    public Rectangle boxCollider;
}

public class PhysicsBody : Component
{

    public Vector2 acceleration = Vector2.Zero;

    public Vector2 velocity = Vector2.Zero;

    public Vector2 gravity = new Vector2(0, 20f);

    public Gravity UseGravity;

    public enum Gravity
    {
        enabled,
        disabled
    }
}

public enum AirState
{
    inAir,
    grounded
}

public class Animator : Component
{
    private int _frame;
    private float _timer;
    private int _maxTime = 2;

    public int frameStart
    {
        get { return _frame; }
        set { _frame = value; }

    }

    public void PlayAnimation(Texture2D spriteSheet, int direction, int maxFrames, Vector2 position, int animationSpeed)
    {
        _timer += Raylib.GetFrameTime() * animationSpeed;

        if (_timer >= _maxTime)
        {
            _timer = 0;
            _frame++;
        }
        _frame %= maxFrames;

        Raylib.DrawTextureRec(spriteSheet, new Rectangle(_frame * spriteSheet.Width / maxFrames, 0, spriteSheet.Width / maxFrames * direction, spriteSheet.Height), position, Color.White);
    }
}