namespace svagui.abstraction
{
    public readonly struct Rect(Vector2 pos, Vector2 size)
    {
        public readonly Vector2 Pos = pos;
        public readonly Vector2 Size = size;
    }
}
