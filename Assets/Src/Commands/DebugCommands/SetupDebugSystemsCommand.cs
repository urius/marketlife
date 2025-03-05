using Src.Systems.DebugSystems;

namespace Src.Commands.DebugCommands
{
    public struct SetupDebugSystemsCommand
    {
        public void Execute()
        {
            new HandleDebugKeyboardShortcutsSystem().Start();
        }
    }
}
