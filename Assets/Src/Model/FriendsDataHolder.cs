using System;

public class FriendsDataHolder
{
    public static FriendsDataHolder Instance => _instance.Value;
    private static readonly Lazy<FriendsDataHolder> _instance = new Lazy<FriendsDataHolder>();

    public int InGameFriendsCount => 1; //TODO inplement
}
