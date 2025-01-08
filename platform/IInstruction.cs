using svagui.abstraction;
using svagui.extensions;
using svagui.extensions.instructions;

namespace svagui.platform
{
    public interface IInstruction
    {
        void Execute<P>(P platform) where P : IGUIPlatform;
    }

    #region ID Stack Manipulation
    public readonly struct PushID(long id) : IInstruction
    {
        public readonly long ID => id;

        public PushID(string name) : this(name.GetHashCode())
        {}

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().IDs.Push(ID);
        }
    }

    public readonly struct PopID : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().IDs.Pop();
        }
    }
    #endregion

    #region Interaction Manipulation
    public readonly struct SetDraggedID(long id) : IInstruction
    {
        public readonly long ID => id;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().DraggedID = ID;
        }
    }

    public readonly struct CheckIfElementDragged : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var env = platform.GetEnvironment();
            var id = env.GetState().Interact.ID;
            var answer = env.DraggedID.HasValue && env.DraggedID.Value == id;
            platform.Run(new PushAnswer(answer));
        }
    }

    public readonly struct UnsetDragged : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().DraggedID = null;
        }
    }

    public readonly struct PushAnswer(bool answer) : IInstruction
    {
        public readonly bool Answer => answer;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().Answers.Push(Answer);
        }
    }

    public readonly struct PopAnswer : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().Answers.Pop();
        }
    }

    public readonly struct PopTwoAndPushIfBothTrue : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().Answers.TryPop(out var a);
            platform.GetEnvironment().Answers.TryPop(out var b);
            platform.GetEnvironment().Answers.Push(a && b);
        }
    }

    public readonly struct PopTwoAndPushIfEitherTrue : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().Answers.TryPop(out var a);
            platform.GetEnvironment().Answers.TryPop(out var b);
            platform.GetEnvironment().Answers.Push(a || b);
        }
    }

    public readonly struct PopAndPushNegated : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().Answers.TryPop(out var a);
            platform.GetEnvironment().Answers.Push(!a);
        }
    }
    #endregion

    #region Layout Stack Manipulation
    public readonly struct PushLayoutDirection(ELayoutDirection dir) : IInstruction
    {
        public readonly ELayoutDirection Direction => dir;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().LayoutDirections.Push(Direction);
        }
    }

    public readonly struct PopLayoutDirection : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().LayoutDirections.Pop();
        }
    }
    #endregion

    #region Layout Subdivision Stack Manipulation
    public readonly struct PushSubdivision() : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().LayoutSubdivisions.Push([]);
        }
    }

    public readonly struct AddAbsoluteSubdivision(int size) : IInstruction
    {
        public readonly int Size => size;
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            if (platform.GetEnvironment().LayoutSubdivisions.TryPeek(out var subs))
            {
                subs.Add(size);
            }
        }
    }

    public readonly struct AddRelativeSubdivision(int size) : IInstruction
    {
        public readonly int Size => size;
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            if (platform.GetEnvironment().LayoutSubdivisions.TryPeek(out var subs))
            {
                subs.Add(-size);
            }
        }
    }

    public readonly struct PushSubdivIndex(int index) : IInstruction
    {
        public readonly int Index => index;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            if (platform.GetEnvironment().LayoutSubdivisionResults.TryPeek(out var results))
            {
                if (Index >= 0 && Index < results.Count)
                {
                    var state = platform.GetEnvironment().GetState();
                    
                    new PushPositionAndSize(
                        results[Index].Pos,
                        results[Index].Size).Execute(platform);
                }
            }
        }
    }

    public readonly struct PopSubdivIndex : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            new PopPositionAndSize().Execute(platform);
        }
    }

    public readonly struct CalcSubdivision : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var divs = new List<Rect>();
            var state = platform.GetEnvironment().GetState();

            var hor = state.Layout.Direction == ELayoutDirection.Horizontal;
            var full = hor ? state.Coords.Size.X : state.Coords.Size.Y;
            var parts = 0.0f;

            foreach (var div in state.Layout.Subdivisions)
            {
                if (div > 0)
                {
                    full -= div;
                }
                else if (div < 0)
                {
                    parts += -div;
                }
            }

            var pos = state.Coords.Position;
            var size = state.Coords.Size;

            foreach (var div in state.Layout.Subdivisions)
            {
                if (div > 0)
                {
                    size = hor ? new Vector2(div, size.Y) : new Vector2(size.X, div);
                }
                else if (div < 0)
                {
                    var ratiod = full / parts * -div;
                    size = hor ? new Vector2(ratiod, size.Y) : new Vector2(size.X, ratiod);
                }
                divs.Add(new Rect(pos, size));
                pos += hor ? new Vector2(size.X, 0) : new Vector2(0, size.Y);
            }

            platform.GetEnvironment().LayoutSubdivisionResults.Push(divs);
        }
    }

    public readonly struct PopSubdivision : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().LayoutSubdivisions.Pop();
            platform.GetEnvironment().LayoutSubdivisionResults.Pop();
        }
    }
    #endregion

    #region Primary Color Stack Manipulation
    public readonly struct PushPrimaryColor(Color color) : IInstruction
    {
        public readonly Color Color { get; } = color;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().PrimaryColors.Push(Color);
        }
    }

    public readonly struct PopPrimaryColor : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().PrimaryColors.Pop();
        }
    }
    #endregion

    #region Secondary Color Stack Manipulation
    public readonly struct PushSecondaryColor(Color color) : IInstruction
    {
        public readonly Color Color { get; } = color;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().SecondaryColors.Push(Color);
        }
    }

    public readonly struct PopSecondaryColor : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().SecondaryColors.Pop();
        }
    }
    #endregion

    #region Draw Mode Stack Manipulation
    public readonly struct PushDrawMode(EDrawMode mode) : IInstruction
    {
        public readonly EDrawMode DrawMode { get; } = mode;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().DrawModes.Push(DrawMode);
        }
    }

    public readonly struct PopDrawMode : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().DrawModes.Pop();
        }
    }
    #endregion

    #region Position Stack Manipulation
    public readonly struct PushPosition(Vector2 position) : IInstruction
    {
        public readonly Vector2 Position { get; } = position;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().Positions.Push(Position);
        }
    }

    public readonly struct PushAddPositionMouseDelta : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var env = platform.GetEnvironment();
            var state = env.GetState();
            env.Positions.Push(state.Coords.Position + env.MouseDelta);
        }
    }

    public readonly struct PushAddPosition(Vector2 delta) : IInstruction
    {
        public readonly Vector2 Delta { get; } = delta;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var state = platform.GetEnvironment().GetState();
            platform.GetEnvironment().Positions.Push(state.Coords.Position + Delta);
        }
    }

    public readonly struct PushMult1Position(float mult) : IInstruction
    {
        public readonly float Mult { get; } = mult;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var state = platform.GetEnvironment().GetState();
            platform.GetEnvironment().Positions.Push(state.Coords.Position * Mult);
        }
    }

    public readonly struct PushMult2Position(Vector2 mult) : IInstruction
    {
        public readonly Vector2 Mult { get; } = mult;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var state = platform.GetEnvironment().GetState();
            platform.GetEnvironment().Positions.Push(new Vector2(
                state.Coords.Position.X * Mult.X,
                state.Coords.Position.Y * Mult.Y));
        }
    }

    public readonly struct PopPosition : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().Positions.Pop();
        }
    }
    #endregion

    #region Size Stack Manipulation
    public readonly struct PushSize(Vector2 size) : IInstruction
    {
        public readonly Vector2 Size { get; } = size;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().Sizes.Push(Size);
        }
    }
    public readonly struct PushAddSize(Vector2 delta) : IInstruction
    {
        public readonly Vector2 Delta { get; } = delta;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var state = platform.GetEnvironment().GetState();
            platform.GetEnvironment().Sizes.Push(state.Coords.Size + Delta);
        }
    }

    public readonly struct PushWidth(float width) : IInstruction
    {
        public readonly float Width { get; } = width;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var size = platform.GetEnvironment().GetState().Coords.Size;
            platform.GetEnvironment().Sizes.Push(new Vector2(Width, size.Y));
        }
    }

    public readonly struct PushHeight(float height) : IInstruction
    {
        public readonly float Height { get; } = height;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var size = platform.GetEnvironment().GetState().Coords.Size;
            platform.GetEnvironment().Sizes.Push(new Vector2(size.X, height));
        }
    }

    public readonly struct PopSize : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().Sizes.Pop();
        }
    }
    #endregion

    #region Position and Size Stack Manipulation
    public readonly struct PushPositionAndSize(Vector2 pos, Vector2 size) : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var env = platform.GetEnvironment();
            env.Positions.Push(pos);
            env.Sizes.Push(size);
        }
    }

    public readonly struct PopAndAddPosition(Vector2 delta) : IInstruction
    {
        public readonly Vector2 Delta => delta;
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var env = platform.GetEnvironment();
            var pos = env.Positions.Pop();
            env.Positions.Push(pos + Delta);
        }
    }

    public readonly struct PopPositionAndSize : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var env = platform.GetEnvironment();
            env.Positions.Pop();
            env.Sizes.Pop();
        }
    }
    #endregion

    #region Radius Stack Manipulation
    public readonly struct PushRadius(float radius) : IInstruction
    {
        public readonly float Radius { get; } = radius;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().Radii.Push(Radius);
        }
    }

    public readonly struct PopRadius : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().Radii.Pop();
        }
    }
    #endregion

    #region Thickness Stack Manipulation
    public readonly struct PushThickness(float thickness) : IInstruction
    {
        public readonly float Thickness { get; } = thickness;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().Thicknesses.Push(Thickness);
        }
    }

    public readonly struct PopThickness : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().Thicknesses.Pop();
        }
    }
    #endregion

    #region Font Stack Manipulation
    public readonly struct PushFont(string font) : IInstruction
    {
        public readonly string Font { get; } = font;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().Fonts.Push(Font);
        }
    }

    public readonly struct PopFont : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().Fonts.Pop();
        }
    }
    #endregion

    #region Text Stack Manipulation
    public readonly struct PushText(string text) : IInstruction
    {
        public readonly string Text { get; } = text;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().Texts.Push(Text);
        }
    }

    public readonly struct PopText : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().Texts.Pop();
        }
    }
    #endregion

    #region Font Size Stack Manipulation
    public readonly struct PushFontSize(int fontSize) : IInstruction
    {
        public readonly int FontSize { get; } = fontSize;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().FontSizes.Push(FontSize);
        }
    }

    public readonly struct PopFontSize : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().FontSizes.Pop();
        }
    }
    #endregion

    #region Font Style Stack Manipulation
    public readonly struct PushFontStyle(EFontStyle fontStyle) : IInstruction
    {
        public readonly EFontStyle FontStyle { get; } = fontStyle;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().FontStyles.Push(FontStyle);
        }
    }

    public readonly struct PopFontStyle : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().FontStyles.Pop();
        }
    }
    #endregion

    #region Texture Stack Manipulation
    public readonly struct PushTexture(string texture) : IInstruction
    {
        public readonly string Texture { get; } = texture;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().Textures.Push(Texture);
        }
    }

    public readonly struct PopTexture : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().Textures.Pop();
        }
    }
    #endregion

    #region Texture Clip Stack Manipulation
    public readonly struct PushTextureClipPosition(Vector2 pos) : IInstruction
    {
        public readonly Vector2 TextureClipPosition { get; } = pos;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().TextureClipPositions.Push(TextureClipPosition);
        }
    }

    public readonly struct PopTextureClipPosition : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().TextureClipPositions.Pop();
        }
    }

    public readonly struct PushTextureClipSize(Vector2 size) : IInstruction
    {
        public readonly Vector2 TextureClipSize { get; } = size;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().TextureClipSizes.Push(TextureClipSize);
        }
    }

    public readonly struct PopTextureClipSize : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().TextureClipSizes.Pop();
        }
    }

    public readonly struct PushTextureClip(Vector2 pos, Vector2 size) : IInstruction
    {
        public readonly Vector2 TextureClipPosition { get; } = pos;
        public readonly Vector2 TextureClipSize { get; } = size;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().TextureClipPositions.Push(TextureClipPosition);
            platform.GetEnvironment().TextureClipSizes.Push(TextureClipSize);
        }
    }

    public readonly struct PopTextureClip : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetEnvironment().TextureClipPositions.Pop();
            platform.GetEnvironment().TextureClipSizes.Pop();
        }
    }
    #endregion

    #region Drawing Instructions
    public readonly struct BaseDrawRectangle : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var state = platform.GetEnvironment().GetState();
            platform.DrawRectangle(
                state.Coords.Position,
                state.Coords.Size,
                state.Colors.PrimaryColor,
                state.Colors.SecondaryColor,
                state.Coords.Thickness,
                state.Coords.Radius);
        }
    }
    public readonly struct BaseDrawCircle : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var state = platform.GetEnvironment().GetState();
            platform.DrawCircle(
                state.Coords.Position,
                state.Coords.Radius,
                state.Colors.PrimaryColor,
                state.Colors.SecondaryColor,
                state.Coords.Thickness);
        }
    }
    public readonly struct BaseDrawLine : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var state = platform.GetEnvironment().GetState();
            platform.DrawLine(
                state.Coords.Position,
                state.Coords.Size,
                state.Colors.SecondaryColor,
                state.Coords.Thickness);
        }
    }
    public readonly struct BaseDrawText : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var state = platform.GetEnvironment().GetState();
            platform.DrawText(
                state.Texts.Font,
                state.Texts.Text,
                state.Texts.FontSize,
                state.Coords.Position,
                state.Colors.PrimaryColor,
                state.Colors.SecondaryColor,
                state.Coords.Thickness);
        }
    }
    public readonly struct BaseDrawTexture : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var state = platform.GetEnvironment().GetState();
            platform.DrawTexture(
                state.Textures.Texture,
                state.Coords.Position,
                state.Coords.Radius,
                state.Textures.TextureClipPosition,
                state.Textures.TextureClipSize,
                state.Colors.PrimaryColor);
        }
    }
    #endregion

    #region Context Instructions
    public readonly struct PushContext(string contextName) : IInstruction
    {
        public readonly string ContextName => contextName;
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetContexts().PushContext(ContextName);
        }
    }

    public readonly struct PopContext : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.GetContexts().PopContext();
        }
    }

    public readonly struct PopAnswerAndBranchContexts(string ifContext, string elseContext) : IInstruction
    {
        public readonly string IfContext => ifContext;
        public readonly string ElseContext => elseContext;
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var ctx = platform.GetEnvironment().Answers.PopOrDefault(false) ? IfContext : ElseContext;
            platform.Run(new RunContext(ctx));
        }
    }

    public readonly struct PopAnswerAndRunContextIfTrue(string ifContext) : IInstruction
    {
        public readonly string IfContext => ifContext;
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            if (platform.GetEnvironment().Answers.PopOrDefault(false))
            {
                platform.Run(new RunContext(IfContext));
            }
        }
    }

    public readonly struct PopAnswerAndSetDraggedIfTrue : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            if (platform.GetEnvironment().Answers.PopOrDefault(false))
            {
                if (!platform.GetEnvironment().DraggedID.HasValue)
                {
                    var id = platform.GetEnvironment().IDs.PeekOrDefault(0);
                    if (id != 0)
                    {
                        platform.Run(new SetDraggedID(id));
                    }
                }
            }
        }
    }

    public readonly struct PopAnswerAndUndragIfTrue : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            if (platform.GetEnvironment().Answers.PopOrDefault(false))
            {
                if (platform.GetEnvironment().DraggedID.HasValue)
                {
                    platform.Run(new UnsetDragged());
                }
            }
        }
    }

    public readonly struct CheckLeftMouseAndUndragIfFalse : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.Run(
                new CheckIfMouseLeftDown(),
                new PopAndPushNegated(),
                new PopAnswerAndUndragIfTrue()
            );
        }
    }

    public readonly struct RunContext(string context) : IInstruction
    {
        public readonly string Context => context;
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var instructions = platform.GetContexts().GetContext(Context);
            platform.RunInstructions(instructions);
        }
    }
    #endregion

    #region Mouse Input Offset Manipulation
    public readonly struct SetMouseInputOffset(Vector2 offset) : IInstruction
    {
        public readonly Vector2 Offset => offset;
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.SetMouseInputOffset(Offset);
        }
    }

    public readonly struct UnsetMouseInputOffset : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.SetMouseInputOffset(new Vector2(0, 0));
        }
    }
    #endregion

    #region Render Surface Manipulation
    public readonly struct SetRenderSurface(string surface) : IInstruction
    {
        public readonly string Surface => surface;
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.SetRenderSurface(surface);
        }
    }

    public readonly struct UnsetRenderSurface : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.UnsetRenderSurface();
        }
    }

    public readonly struct SetRenderSurfaceOffset(Vector2 offset) : IInstruction
    {
        public readonly Vector2 Offset => offset;
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.SetRenderSurfaceOffset(Offset);
        }
    }

    public readonly struct UnsetRenderSurfaceOffset : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.SetRenderSurfaceOffset(new Vector2(0, 0));
        }
    }

    public readonly struct SetRenderSurfaceSize(Vector2 offset) : IInstruction
    {
        public readonly Vector2 Offset => offset;
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.SetRenderSurfaceSize(Offset);
        }
    }

    public readonly struct UnsetRenderSurfaceSize : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.SetRenderSurfaceSize(new Vector2(0, 0));
        }
    }
    #endregion
}
