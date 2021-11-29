using System;
using System.Collections.Generic;

public class AdvertViewStateModel
{
    public static readonly AdvertViewStateModel Instance = new AdvertViewStateModel();

    public event Action<AdvertTargetType> WatchStateChanged = delegate { };

    private Dictionary<AdvertTargetType, AdvertWatchState> _advertStates = new Dictionary<AdvertTargetType, AdvertWatchState>();
    private AdvertTargetType _lastPreparedTarget;

    public void PrepareTarget(AdvertTargetType target)
    {
        _advertStates[target] = AdvertWatchState.Prepared;
        _lastPreparedTarget = target;
        WatchStateChanged(_lastPreparedTarget);
    }

    public void MarkCurrentAsWatched()
    {
        _advertStates[_lastPreparedTarget] = AdvertWatchState.Watched;
        WatchStateChanged(_lastPreparedTarget);
    }

    public void ResetTarget(AdvertTargetType target)
    {
        if (_advertStates.ContainsKey(target))
        {
            _advertStates[target] = AdvertWatchState.Default;
        }
        if (_lastPreparedTarget == target)
        {
            _lastPreparedTarget = AdvertTargetType.None;
        }
    }

    public AdvertWatchState GetWatchState(AdvertTargetType target)
    {
        if (_advertStates.ContainsKey(target))
        {
            return _advertStates[target];
        }

        return AdvertWatchState.Default;
    }

    public bool IsWatched(AdvertTargetType target)
    {
        return GetWatchState(target) == AdvertWatchState.Watched;
    }

    public AdvertTargetType GetAdvertTargetTypeByDailyBonusDayNum(int dayNum)
    {
        return dayNum switch
        {
            1 => AdvertTargetType.DailyDouble1,
            2 => AdvertTargetType.DailyDouble2,
            3 => AdvertTargetType.DailyDouble3,
            4 => AdvertTargetType.DailyDouble4,
            5 => AdvertTargetType.DailyDouble5,
            _ => AdvertTargetType.None,
        };
    }
}

public enum AdvertTargetType
{
    None,
    OfflineProfitX2,
    BankGold,
    BankCash,
    DailyDouble1,
    DailyDouble2,
    DailyDouble3,
    DailyDouble4,
    DailyDouble5,
}

public enum AdvertWatchState
{
    Default,
    Prepared,
    Watched,
}
