using svagui.abstraction;
using svagui.platform;

namespace svagui.extensions.instructions
{
    public readonly struct DrawRectangle(
        Vector2? position = null,
        Vector2? size = null,
        Color? fill = null,
        Color? outline = null,
        float? thickness = null,
        float? radius = null) : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var insts = new List<IInstruction>();
            if (position.HasValue) insts.Add(new PushPosition(position.Value));
            if (size.HasValue) insts.Add(new PushSize(size.Value));
            if (fill.HasValue) insts.Add(new PushPrimaryColor(fill.Value));
            if (outline.HasValue) insts.Add(new PushSecondaryColor(outline.Value));
            if (thickness.HasValue) insts.Add(new PushThickness(thickness.Value));
            if (radius.HasValue) insts.Add(new PushRadius(radius.Value));
            insts.Add(new BaseDrawRectangle());
            if (position.HasValue) insts.Add(new PopPosition());
            if (size.HasValue) insts.Add(new PopSize());
            if (fill.HasValue) insts.Add(new PopPrimaryColor());
            if (outline.HasValue) insts.Add(new PopSecondaryColor());
            if (thickness.HasValue) insts.Add(new PopThickness());
            if (radius.HasValue) insts.Add(new PopRadius());
            platform.RunInstructions(insts);
        }
    }

    public readonly struct DrawCircle(
        Vector2? center = null,
        float? radius = null,
        Color? fill = null,
        Color? outline = null,
        float? thickness = null) : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var insts = new List<IInstruction>();
            if (center.HasValue) insts.Add(new PushPosition(center.Value));
            if (radius.HasValue) insts.Add(new PushRadius(radius.Value));
            if (fill.HasValue) insts.Add(new PushPrimaryColor(fill.Value));
            if (outline.HasValue) insts.Add(new PushSecondaryColor(outline.Value));
            if (thickness.HasValue) insts.Add(new PushThickness(thickness.Value));
            insts.Add(new BaseDrawCircle());
            if (center.HasValue) insts.Add(new PopPosition());
            if (radius.HasValue) insts.Add(new PopRadius());
            if (fill.HasValue) insts.Add(new PopPrimaryColor());
            if (outline.HasValue) insts.Add(new PopSecondaryColor());
            if (thickness.HasValue) insts.Add(new PopThickness());
            platform.RunInstructions(insts);
        }
    }

    public readonly struct DrawLine(
        Vector2 start,
        Vector2 end,
        Color? outline = null,
        float? thickness = null) : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var insts = new List<IInstruction>();
            insts.Add(new PushPosition(start));
            insts.Add(new PushSize(end - start));
            if (outline.HasValue) insts.Add(new PushSecondaryColor(outline.Value));
            if (thickness.HasValue) insts.Add(new PushThickness(thickness.Value));
            insts.Add(new BaseDrawLine());
            insts.Add(new PopSize());
            insts.Add(new PopPosition());
            if (outline.HasValue) insts.Add(new PopSecondaryColor());
            if (thickness.HasValue) insts.Add(new PopThickness());
            platform.RunInstructions(insts);
        }
    }

    public readonly struct DrawText(
        string text,
        string? font = null,
        int? fontSize = null,
        Vector2? position = null,
        Color? fill = null,
        Color? outline = null,
        float? thickness = null) : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var insts = new List<IInstruction>();
            insts.Add(new PushText(text));
            if (font != null) insts.Add(new PushFont(font));
            if (fontSize.HasValue) insts.Add(new PushFontSize(fontSize.Value));
            if (position.HasValue) insts.Add(new PushPosition(position.Value));
            if (fill.HasValue) insts.Add(new PushPrimaryColor(fill.Value));
            if (outline.HasValue) insts.Add(new PushSecondaryColor(outline.Value));
            if (thickness.HasValue) insts.Add(new PushThickness(thickness.Value));
            insts.Add(new BaseDrawText());
            insts.Add(new PopText());
            if (font != null) insts.Add(new PopFont());
            if (fontSize.HasValue) insts.Add(new PopFontSize());
            if (position.HasValue) insts.Add(new PopPosition());
            if (fill.HasValue) insts.Add(new PopPrimaryColor());
            if (outline.HasValue) insts.Add(new PopSecondaryColor());
            if (thickness.HasValue) insts.Add(new PopThickness());
            platform.RunInstructions(insts);
        }
    }

}
