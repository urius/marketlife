public struct SetupExternalDebugDataCommand
{
    public void Execute(string uid)
    {
        var playerModelHolder = PlayerModelHolder.Instance;
        playerModelHolder.SetUid(uid);

        var mockDataProvider = MockDataProvider.Instance;
        new SetVkFriendsDataCommand().Execute(mockDataProvider.GetMockFriends());
    }
}
