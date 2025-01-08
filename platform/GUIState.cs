using svagui.abstraction;

using System.Security.Cryptography.X509Certificates;

namespace svagui.platform
{
    public readonly struct LayoutState(
        ELayoutDirection direction,
        List<int> subdiv,
        List<Rect> subdivResults)
    {
        public ELayoutDirection Direction => direction;
        public List<int> Subdivisions => subdiv;
        public List<Rect> SubdivResults => subdivResults;
    }

    public readonly struct InteractionState(
        Vector2 mouseDelta,
        long id,
        long? dragged,
        bool? answer)
    {
        public Vector2 MouseDelta => mouseDelta;
        public long ID => id;
        public long? DraggedID => dragged;
        public bool? Answer => answer;
    }

    public readonly struct ColorState(
        Color primaryColor,
        Color secondaryColor,
        EDrawMode drawMode)
    {
        public Color PrimaryColor => primaryColor;
        public Color SecondaryColor => secondaryColor;
        public EDrawMode DrawMode => drawMode;
    }

    public readonly struct CoordState(
        Vector2 position,
        Vector2 size,
        float radius,
        float thickness)
    {
        public Vector2 Position => position;
        public Vector2 Size => size;
        public float Radius => radius;
        public float Thickness => thickness;
    }

    public readonly struct TextState(
        string font,
        string text,
        int fontSize,
        EFontStyle style)
    {
        public string Font => font;
        public string Text => text;
        public int FontSize => fontSize;
        public EFontStyle Style => style;
    }

    public readonly struct TextureState(
        string texture,
        Vector2 textureClipPosition,
        Vector2 textureClipSize)
    {
        public string Texture => texture;
        public Vector2 TextureClipPosition => textureClipPosition;
        public Vector2 TextureClipSize => textureClipSize;
    }

    public readonly struct GUIState(
        LayoutState layout,
        InteractionState interacts,
        ColorState colors,
        CoordState coords,
        TextState texts,
        TextureState textures)
    {
        public LayoutState Layout => layout;
        public InteractionState Interact => interacts;
        public ColorState Colors => colors;
        public CoordState Coords => coords;
        public TextState Texts => texts;
        public TextureState Textures => textures;
    }
}
