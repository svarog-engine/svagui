namespace svagui.extensions
{
    public static class StackExtensions
    {
        public static T PopOrDefault<T>(this Stack<T> stack, T defaultValue)
        {
            if (stack.TryPop(out var value)) { return value; }
            return defaultValue;
        }

        public static T PeekOrDefault<T>(this Stack<T> stack, T defaultValue)
        {
            if (stack.TryPeek(out var value)) { return value; }
            return defaultValue;
        }
    }
}
