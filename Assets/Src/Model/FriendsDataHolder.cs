using System;
using System.Collections.Generic;
using System.Linq;

public class FriendsDataHolder
{
    public static FriendsDataHolder Instance => _instance.Value;
    private static readonly Lazy<FriendsDataHolder> _instance = new Lazy<FriendsDataHolder>();

    public event Action FriendsDataWasSetup = delegate { };

    private FriendData[] _friends;

    public FriendsDataHolder()
    {
    }

    public int InGameFriendsCount => _friends?.Length ?? 0;
    public IReadOnlyList<FriendData> Friends => _friends ?? Array.Empty<FriendData>();
    public bool FriendsDataIsSet => _friends != null;

    public void SetupFriendsData(IEnumerable<FriendData> friendsData)
    {
        var friendsDataArr = friendsData.ToArray();
        Array.Sort(friendsDataArr);
        _friends = friendsDataArr;
        FriendsDataWasSetup();
    }

    public FriendData GetFriendData(string uid)
    {
        return _friends.FirstOrDefault(d => d.Uid == uid);
    }
}

public class FriendData : IComparable<FriendData>
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

    public int LastVisitTime { get; private set; }
    public UserModel UserModel { get; private set; }
    public bool IsUserModelLoaded => UserModel != null;
    public string FullName => $"{FirstName} {LastName}";

    public int CompareTo(FriendData other)
    {
        return other.LastVisitTime - LastVisitTime;
    }

    public void SetLastVisitTime(int lastVisitTime)
    {
        LastVisitTime = Math.Max(LastVisitTime, lastVisitTime);
    }

    public void SetUserModel(UserModel userModel)
    {
        SetLastVisitTime(userModel.StatsData.LastVisitTimestamp);
        UserModel = userModel;
    }
}

public static class FriendDataExtensions
{
    public static bool IsInactive(this FriendData friendData)
    {
        if (friendData.LastVisitTime <= 0) return true;
        var thresholdHours = GameConfigManager.Instance.MainConfig.FriendInactivityThresholdHours;
        var startPlayTime = GameStateModel.Instance.StartGameServerTime;
        var hoursSinceFriendsLastVisit = Math.Max(0, startPlayTime - friendData.LastVisitTime) / 3600f;
        return hoursSinceFriendsLastVisit > thresholdHours;
    }
}