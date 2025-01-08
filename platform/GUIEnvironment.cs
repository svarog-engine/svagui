using svagui.abstraction;
using svagui.extensions;

namespace svagui.platform
{
    public enum ELayoutDirection
    {
        Horizontal,
        Vertical
    }

    public enum EDrawMode
    {
        Outline,
        Fill,
        Both
    }

    public enum EFontStyle : byte
    {
        Regular = 0,
        Bold = 1,
        Italic = 2,
    }

    public class GUIEnvironment
    {
        public long? DraggedID { get; set; } = null;
        public Vector2 MouseDelta { get; set; } = new Vector2(0, 0);
        public Stack<long> IDs { get; } = new();
        public Stack<bool> Answers { get; set; } = new();
        public Stack<ELayoutDirection> LayoutDirections { get; } = new();
        public Stack<List<int>> LayoutSubdivisions { get; } = new();
        public Stack<List<Rect>> LayoutSubdivisionResults { get; } = new();
        public Stack<Color> PrimaryColors { get; } = new();
        public Stack<Color> SecondaryColors { get; } = new();
        public Stack<EDrawMode> DrawModes { get; } = new();
        public Stack<Vector2> Positions { get; } = new();
        public Stack<Vector2> Sizes { get; } = new();
        public Stack<float> Radii { get; } = new();
        public Stack<float> Thicknesses { get; } = new();
        public Stack<string> Fonts { get; } = new();
        public Stack<string> Texts { get; } = new();
        public Stack<int> FontSizes { get; } = new();
        public Stack<EFontStyle> FontStyles { get; } = new();
        public Stack<string> Textures { get; } = new();
        public Stack<Vector2> TextureClipPositions { get; } = new();
        public Stack<Vector2> TextureClipSizes { get; } = new();
        public Dictionary<string, object> Data { get; internal set; } = new();

        public GUIState GetState()
        {
            return new GUIState(
                new LayoutState(
                    LayoutDirections.PeekOrDefault(ELayoutDirection.Vertical),
                    LayoutSubdivisions.PeekOrDefault([]),
                    LayoutSubdivisionResults.PeekOrDefault([])
                ),
                new InteractionState(
                    MouseDelta,
                    IDs.PeekOrDefault(0),
                    DraggedID,
                    Answers.PeekOrDefault(false)
                ),
                new ColorState(
                    PrimaryColors.PeekOrDefault(new Color(255, 255, 255, 255)),
                    SecondaryColors.PeekOrDefault(new Color(0, 0, 0, 0)),
                    DrawModes.PeekOrDefault(EDrawMode.Both)
                ),
                new CoordState(
                    Positions.PeekOrDefault(new Vector2(0, 0)),
                    Sizes.PeekOrDefault(new Vector2(1, 1)),
                    Radii.PeekOrDefault(1.0f),
                    Thicknesses.PeekOrDefault(1.0f)
                ),
                new TextState(
                    Fonts.PeekOrDefault(""),
                    Texts.PeekOrDefault(""),
                    FontSizes.PeekOrDefault(13),
                    FontStyles.PeekOrDefault(EFontStyle.Regular)
                ),
                new TextureState(
                    Textures.PeekOrDefault(""),
                    TextureClipPositions.PeekOrDefault(new Vector2(0, 0)),
                    TextureClipSizes.PeekOrDefault(new Vector2(0, 0))
                )
            );
        }
    }
}
