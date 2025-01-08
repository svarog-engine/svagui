using svagui.platform;

namespace svagui.extensions.instructions
{
    public readonly struct CheckIfCurrentHovered : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var state = platform.GetEnvironment().GetState();
            var pos = state.Coords.Position;
            var size = state.Coords.Size;

            var mouse = platform.GetInputState().MousePosition + platform.GetMouseInputOffset();
            var answer = mouse.X >= pos.X &&
                mouse.X < pos.X + size.X &&
                mouse.Y >= pos.Y &&
                mouse.Y < pos.Y + size.Y;

            platform.Run(new PushAnswer(answer));
        }
    }

    public readonly struct CheckIfCurrentLeftClicked : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var state = platform.GetEnvironment().GetState();
            var pos = state.Coords.Position;
            var size = state.Coords.Size;
            var mouse = platform.GetInputState().MousePosition + platform.GetMouseInputOffset();
            var answer = mouse.X >= pos.X &&
                mouse.X < pos.X + size.X &&
                mouse.Y >= pos.Y &&
                mouse.Y < pos.Y + size.Y;

            var click = platform.GetInputState().IsLeftClicked && platform.GetInputState().MouseInputsActive;
            platform.Run(new PushAnswer(answer && click));
        }
    }

    public readonly struct CheckIfMouseLeftDown : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.Run(new PushAnswer(platform.GetInputState().IsMouseDown && platform.GetInputState().MouseInputsActive));
        }
    }

    public readonly struct CheckIfMouseLeftClicked : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.Run(new PushAnswer(platform.GetInputState().IsLeftClicked && platform.GetInputState().MouseInputsActive));
        }
    }

    public readonly struct CheckIfCurrentRightClicked : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            var state = platform.GetEnvironment().GetState();
            var pos = state.Coords.Position;
            var size = state.Coords.Size;

            var mouse = platform.GetInputState().MousePosition + platform.GetMouseInputOffset();
            var answer = mouse.X >= pos.X &&
                mouse.X < pos.X + size.X &&
                mouse.Y >= pos.Y &&
                mouse.Y < pos.Y + size.Y;

            platform.Run(new PushAnswer(answer && platform.GetInputState().IsRightClicked && platform.GetInputState().MouseInputsActive));
        }
    }

    public readonly struct CheckIfMouseRightClicked : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.Run(new PushAnswer(platform.GetInputState().IsRightClicked && platform.GetInputState().MouseInputsActive));
        }
    }

    public readonly struct PopAndSetMouseInputActive : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.SetMouseInputsEnabled(platform.GetEnvironment().Answers.PopOrDefault(false));
        }
    }

    public readonly struct EnableMouseInputActive : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.SetMouseInputsEnabled(true);
        }
    }

    public readonly struct DisableMouseInputActive : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.SetMouseInputsEnabled(false);
        }
    }

    public readonly struct SetMouseInputActive(bool value) : IInstruction
    {
        public void Execute<P>(P platform) where P : IGUIPlatform
        {
            platform.SetMouseInputsEnabled(value);
        }
    }
}
