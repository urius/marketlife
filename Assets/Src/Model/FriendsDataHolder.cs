using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

public class FriendsDataHolder
{
    public static FriendsDataHolder Instance = new FriendsDataHolder();

    public event Action FriendsDataWasSetup = delegate { };

    private UniTaskCompletionSource _friendsDataSetupTcs = new UniTaskCompletionSource();

    private FriendData[] _friends;

    public FriendsDataHolder()
    {
    }

    public int InGameFriendsCount => _friends != null ? _friends.Where(f => f.IsApp).Count() : 0;
    public int InGameActiveFriendsCount => _friends != null ? _friends.Where(f => f.IsApp && f.IsActive()).Count() : 0;
    public IReadOnlyList<FriendData> Friends => _friends ?? Array.Empty<FriendData>();
    public bool FriendsDataIsSet => _friends != null;
    public UniTask FriendsDataSetupTask => _friendsDataSetupTcs.Task;

    public void SetupFriendsData(IEnumerable<FriendData> friendsData)
    {
        var friendsDataArr = friendsData.ToArray();
        Array.Sort(friendsDataArr);
        _friends = friendsDataArr;
        FriendsDataWasSetup();
        _friendsDataSetupTcs.TrySetResult();
    }

    public FriendData GetFriendData(string uid)
    {
        return _friends.FirstOrDefault(d => d.Uid == uid);
    }
}

public class FriendData : UserSocialData, IComparable<FriendData>
{
    public readonly bool IsApp;

    public FriendData(string uid, bool isApp, string firstName, string lastName, string picture50Url)
        : base(uid, firstName, lastName, picture50Url)
    {
        IsApp = isApp;
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
    public static bool IsActive(this FriendData friendData)
    {
        return !friendData.IsInactive();
    }
}