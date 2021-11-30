public struct SetupDebugSystemsCommand
{
    public void Execute()
    {
        new HandleDebugKeyboardShortcutsSystem().Start();
    }
}
