namespace svagui.abstraction
{

    public readonly struct Vector2(float x, float y)
    {
        public readonly float X = x;
        public readonly float Y = y;

        public override string ToString() => $"[ {X}, {Y} ]";
        
        public static Vector2 operator-(Vector2 v)
        {
            return new Vector2(-v.X, -v.Y);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2 operator *(Vector2 a, float f)
        {
            return new Vector2(a.X * f, a.Y * f);
        }
    }

}
