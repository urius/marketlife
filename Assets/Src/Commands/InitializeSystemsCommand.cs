public struct InitializeSystemsCommand
{
    public void Execute()
    {
        new EventsHandleSystem().Initialize();
    }
}
