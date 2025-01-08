using svagui.platform;
using svagui.extensions;
using svagui.abstraction;
using System;
using System.Xml.Linq;

namespace svagui.extensions.instructions
{
    public class Palette
    {
        public readonly Color BackgroundFill = new(30, 35, 36, 220);
        public readonly Color TitleBarUnfocusedFill = new(46, 46, 46, 220);
        public readonly Color TitleBarFocusedFill = new(56, 56, 56, 220);
        public readonly Color ButtonUnfocusedFill = new(200, 200, 200, 220);
        public readonly Color ButtonFocusedFill = new(255, 255, 255, 220);
        public readonly Color Text = new(200, 200, 200, 220);
    }

    public readonly struct PushWindow(string name, Palette palette) : IInstruction
    {
        private readonly IInstruction[] block =
        [
            new PushContext("unfocused titlebar"),
            new PushPrimaryColor(palette.TitleBarUnfocusedFill),
            new PopContext(),
            new PushContext("focused titlebar"),
            new PushPrimaryColor(palette.TitleBarFocusedFill),
            new PopContext(),
            new PushContext("unfocused button"),
            new PushPrimaryColor(palette.ButtonUnfocusedFill),
            new PopContext(),
            new PushContext("focused button"),
            new PushPrimaryColor(palette.ButtonFocusedFill),
            new PopContext(),
            new PushContext("drag"),
            new PushAddPositionMouseDelta(),
            new CheckLeftMouseAndUndragIfFalse(),
            new PopContext(),

            new PushID(name),
            new CheckIfElementDragged(),
            new PopAnswerAndRunContextIfTrue("drag"),
            new DrawRectangle(fill: palette.BackgroundFill),
            new CheckIfCurrentHovered(), // check if current window hovered, leave on stack
            new PushHeight(20.0f),
            new CheckIfCurrentLeftClicked(),
            new PopAnswerAndSetDraggedIfTrue(),
            new CheckIfCurrentHovered(),
            new PopAnswerAndBranchContexts("focused titlebar", "unfocused titlebar"),
            new DrawRectangle(),
            new PopPrimaryColor(),
            new PushLayoutDirection(ELayoutDirection.Horizontal),
            new PushSubdivision(),
            new AddRelativeSubdivision(1),
            new AddAbsoluteSubdivision(20),
            new CalcSubdivision(),
            new PushSubdivIndex(0),
            new PushAddPosition(new Vector2(6, 2)),
            new DrawText(name),
            new PopPosition(),
            new PopSubdivIndex(),
            new PushSubdivIndex(1),
            new CheckIfCurrentHovered(),
            new PopAnswerAndBranchContexts("focused button", "unfocused button"),
            new PushAddPosition(new Vector2(4, 5)),
            new DrawCircle(radius: 5.0f),
            new PopPosition(),
            new PopPrimaryColor(),
            new CheckIfCurrentLeftClicked(),
            new PopSubdivIndex(),
            new PopSubdivision(),
            new PopLayoutDirection(),
            new PopSize()
        ];

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.RunInstructions(block);
            var state = platform.GetEnvironment().GetState();

            var newPosition = state.Coords.Position;
            var newVisible = !platform.GetEnvironment().Answers.PopOrDefault(false);
            var windowHovered = platform.GetEnvironment().Answers.PopOrDefault(false);

            var data = platform.GetEnvironment().Data;
            data.Clear();
            data.Add("id", state.Interact.ID);
            data.Add("position", newPosition);
            data.Add("visible", newVisible);
            data.Add("window-hovered", windowHovered);

            // prepare internals
            platform.Run(
                new PushAddPosition(new Vector2(5, 25)),
                new PushAddSize(new Vector2(-10, -30)), // inner frame
                new CheckIfCurrentHovered()
            );

            var contentHovered = platform.GetEnvironment().Answers.PopOrDefault(false);
            data.Add("window-content-hovered", contentHovered);
        }
    }

    public readonly struct PopWindow : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.Run(
                new PopPositionAndSize(), // internal position and size
                new PopPositionAndSize(), // window position and size
                new PopID()
            );
        }
    }

    public readonly struct ConsumeMouseScrollIntoInputDelta(float speed) : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var delta = platform.GetInputState().MouseDelta;
            var off = platform.GetMouseInputOffset();
            var newDelta = new Vector2(off.X, off.Y + delta * speed);
            platform.SetMouseInputOffset(newDelta);
        }
    }

    public readonly struct PushClipView(string name) : IInstruction
    {
        public readonly string ClipName => name;

        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var pos = platform.GetEnvironment().GetState().Coords.Position;
            var size = platform.GetEnvironment().GetState().Coords.Size;
            platform.Run(
                new SetRenderSurface(ClipName),
                new SetRenderSurfaceOffset(new Vector2(-pos.X, -pos.Y)),
                new SetRenderSurfaceSize(size)
            );
        }
    }

    public readonly struct PopClipView() : IInstruction
    { 
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var name = platform.GetRenderSurface();
            var pos = platform.GetRenderSurfaceOffset();
            var size = platform.GetRenderSurfaceSize();
            var offset = platform.GetMouseInputOffset();
            platform.Run(
                new UnsetRenderSurface(),
                new UnsetRenderSurfaceOffset(),
                new UnsetRenderSurfaceSize(),
                new PushPosition(-pos),
                new PushTexture(name),
                new PushTextureClip(offset, size),
                new BaseDrawTexture(),
                new PopPosition(),
                new PopTextureClip(),
                new PopTexture(),
                new UnsetMouseInputOffset()
            );
        }
    }

    public readonly struct DrawLabel(
        string name, 
        string? font = null,
        int? fontSize = null,
        Vector2? position = null,
        Color? fill = null,
        Color? outline = null,
        float? thickness = null) : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var state = platform.GetEnvironment().GetState();
            platform.Run(new DrawText(name, font, fontSize, position, fill, outline, thickness));
            var delta = state.Layout.Direction == ELayoutDirection.Horizontal
                    ? new Vector2(MathF.Round(state.Texts.FontSize * name.Length * 0.66f), 0)
                    : new Vector2(0, state.Texts.FontSize + 4);
            var data = platform.GetEnvironment().Data;
            data["last-label-text"] = name;
            data["last-label-rect"] = new Rect(state.Coords.Position, new Vector2(MathF.Round(state.Texts.FontSize * name.Length * 0.66f), state.Texts.FontSize + 4));
            platform.Run(new PopAndAddPosition(delta));
        }
    }

    public static class WindowingExtensions
    {
        public static void UpdateWindow(this GUIEnvironment env, ref Vector2 position, ref bool visible)
        {
            var data = env.Data;
            visible = (bool)data["visible"];
            position = (Vector2)data["position"];
        }
    }
}
