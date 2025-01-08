using svagui.platform;

namespace svagui.extensions
{
    public static class IGUIPlatformExtensions
    {
        public static void RunInstructions<P, L>(this P self, L instructions)
            where L : IEnumerable<IInstruction>
            where P : IGUIPlatform
        {
            int instructionCount = 0;
            foreach (var instruction in instructions)
            {
                try
                {
                    self.ExecuteInstruction(instruction);
                }
                catch(Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Broke at #{instructionCount} [{instructions.ToArray()[instructionCount]}]: {e.Message}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }

        public static void Run<P>(this P self, params IInstruction[] instructions)
            where P : IGUIPlatform
        {
            int instructionCount = 0;
            foreach (var instruction in instructions)
            {
                try
                {
                    self.ExecuteInstruction(instruction);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Broke at #{instructionCount} {self.GetContexts().CurrentContext ?? ""} [{instructions[instructionCount]}]: {e.Message}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                instructionCount++;
            }
        }
    }
}
