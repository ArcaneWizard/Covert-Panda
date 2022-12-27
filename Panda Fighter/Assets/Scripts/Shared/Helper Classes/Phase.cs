public static class Phase
{
    public static int Idle = 0;
    public static int Running = 1;
    public static int Jumping = 2;
    public static int DoubleJumping = 3;
    public static int Falling = 4;

    public static bool IsMidAir(int p) => (2 <= p && p <= 4);
}