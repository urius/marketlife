public struct SetupExternalDebugDataCommand
{
    public void Execute(string uid)
    {
        var playerModelHolder = PlayerModelHolder.Instance;
        playerModelHolder.SetInitialData(uid, SocialType.Undefined);

        var mockDataProvider = MockDataProvider.Instance;
        new SetVkFriendsDataCommand().Execute(mockDataProvider.GetMockFriends());
    }
}
