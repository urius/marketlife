public struct InitializeSystemsCommand
{
    public void Execute()
    {
        new EventsHandleSystem().Start();
        new SaveDataSystem().Start();
        new HumansControlSystem().Start();
    }
}
