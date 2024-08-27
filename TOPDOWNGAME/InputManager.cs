public class InputManager
{
    public static float GetAxisX()
    {
        if (Raylib.IsKeyDown(KeyboardKey.A) && !Raylib.IsKeyDown(KeyboardKey.D))
            return -1;

        else if (Raylib.IsKeyDown(KeyboardKey.D) && !Raylib.IsKeyDown(KeyboardKey.A))
            return 1;

        return 0;
    }
    public static float GetAxisY()
    {
        if (Raylib.IsKeyDown(KeyboardKey.W) && !Raylib.IsKeyDown(KeyboardKey.S))
            return -1;

        else if (Raylib.IsKeyDown(KeyboardKey.S) && !Raylib.IsKeyDown(KeyboardKey.W))
            return 1;

        return 0;
    }

    private static int _directionDelta = 1;

    public static float GetLastDirectionDelta()
    {
        if (Raylib.IsKeyDown(KeyboardKey.A))
            _directionDelta = 1;

        else if (Raylib.IsKeyDown(KeyboardKey.D))
            _directionDelta = 2;

        else if (Raylib.IsKeyDown(KeyboardKey.W))
            _directionDelta = 3;

        else if (Raylib.IsKeyDown(KeyboardKey.S))
            _directionDelta = 4;

        return _directionDelta;
    }

}

