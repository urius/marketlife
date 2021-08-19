using System;
using System.Collections.Generic;

public class FriendsDataHolder
{
    public static FriendsDataHolder Instance => _instance.Value;
    private static readonly Lazy<FriendsDataHolder> _instance = new Lazy<FriendsDataHolder>();

    public event Action FriendsDataIsSet = delegate { };

    private FriendData[] _friends;

    public FriendsDataHolder()
    {
    }

    public int InGameFriendsCount => _friends?.Length ?? 0;
    public IReadOnlyList<FriendData> Friends => _friends ?? Array.Empty<FriendData>();

    public void SetupFriendsData(FriendData[] friendsData)
    {
        _friends = friendsData;
        FriendsDataIsSet();
    }

}

public class FriendData
{
    public readonly string Uid;
    public readonly bool IsApp;
    public readonly string FirstName;
    public readonly string LastName;
    public readonly string Picture50Url;

    public FriendData(string uid, bool isApp, string firstName, string lastName, string picture50Url)
    {
        Uid = uid;
        IsApp = isApp;
        FirstName = firstName;
        LastName = lastName;
        Picture50Url = picture50Url;
    }
}