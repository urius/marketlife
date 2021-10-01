public struct InitializeSystemsCommand
{
    public void Execute()
    {
        new EventsHandleSystem().Start();
        new PlatformSpecificLogicSystem().Start();
        new SaveDataSystem().Start();
        new HumansControlSystem().Start();
        new TutorialSystem().Start();
        new MusicControlSystem().Start();
    }
}
