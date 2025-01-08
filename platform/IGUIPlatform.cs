using svagui.abstraction;
using svagui.extensions;

namespace svagui.platform
{
    public struct GUIInputState(
        bool mouseInputsActive,
        Vector2 mousePosition,
        bool isLeftClicked,
        bool isRightClicked,
        bool isMouseDown,
        float mouseDelta)
    {
        public bool MouseInputsActive { get; set; } = mouseInputsActive;
        public Vector2 MousePosition { get; set; } = mousePosition;
        public bool IsLeftClicked { get; set; } = isLeftClicked;
        public bool IsRightClicked { get; set; } = isRightClicked;
        public bool IsMouseDown { get; set; } = isMouseDown;
        public float MouseDelta { get; set; } = mouseDelta;
        public GUIInputState() : this(true, new Vector2(0, 0), false, false, false, 0.0f)
        { }
    }

    public class GUIContexts
    {
        private Dictionary<string, IInstruction[]> m_Contexts = new();
        private Stack<string> m_CurrentContext = new();

        private List<IInstruction> m_Instructions = new();
        public bool m_Skip = false;

        public bool Skip => m_Skip;
        public string? CurrentContext => m_CurrentContext.PeekOrDefault(null) ?? null;
        
        public bool DefiningNewContext => CurrentContext != null && !m_Contexts.ContainsKey(CurrentContext);
        public bool IsInContext()
        {
            return m_CurrentContext.Count > 0;
        }

        public void PushContext(string name)
        {
            if (!m_Contexts.ContainsKey(name))
            {
                m_CurrentContext.Push(name);
                m_Instructions.Clear();
            }
            else
            {
                m_Skip = true;
            }
        }

        public void AddToContext(IInstruction instruction)
        {
            if (m_CurrentContext.Count > 0)
            {
                m_Instructions.Add(instruction);
            }
        }

        public void PopContext()
        {
            if (m_CurrentContext.Count > 0)
            {
                var ctx = m_CurrentContext.Pop();
                m_Contexts[ctx] = m_Instructions.ToArray();
            }

            m_Skip = false;
        }

        public IInstruction[] GetContext(string name)
        {
            return m_Contexts[name];
        }
    }

    public interface IGUIPlatform
    {
        GUIInputState GetInputState();
        GUIEnvironment GetEnvironment();
        GUIContexts GetContexts();

        void SetRenderSurface(string name);
        void UnsetRenderSurface();
        string GetRenderSurface();

        void SetMouseInputsEnabled(bool enabled);
        bool GetMouseInputsEnabled();

        void SetMouseInputOffset(Vector2 offset);
        Vector2 GetMouseInputOffset();

        void SetRenderSurfaceOffset(Vector2 offset);
        Vector2 GetRenderSurfaceOffset();
        void SetRenderSurfaceSize(Vector2 offset);
        Vector2 GetRenderSurfaceSize();

        void DrawRectangle(Vector2 position, Vector2 size, Color fill, Color outline, float thickness, float radius);
        void DrawCircle(Vector2 center, float radius, Color fill, Color outline, float thickness);
        void DrawLine(Vector2 position, Vector2 size, Color outline, float thickness);
        void DrawText(string font, string text, int fontSize, Vector2 position, Color fill, Color outline, float thickness);
        void DrawTexture(string texture, Vector2 position, float scale, Vector2 clipPosition, Vector2 clipSize, Color tint);
        void Update(GUIInputState inputState);

        public void ExecuteInstruction(IInstruction instruction)
        {
            var ctxs = GetContexts();
            if (ctxs.IsInContext())
            {
                if (instruction is PopContext && ctxs.DefiningNewContext)
                {
                    instruction.Execute(this);
                }
                else if (ctxs.DefiningNewContext)
                {
                    if (!ctxs.Skip)
                    {
                        ctxs.AddToContext(instruction);
                    }
                }
            }
            else
            {
                if (instruction is PopContext || !ctxs.Skip)
                {
                    instruction.Execute(this);
                }
            }
        }
    }
}
