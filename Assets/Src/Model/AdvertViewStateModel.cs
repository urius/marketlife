using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Src.Model
{
    public class AdvertViewStateModel : IBankAdvertWatchStateProvider
    {
        public static readonly AdvertViewStateModel Instance = new();

        public event Action<AdvertTargetType> WatchStateChanged = delegate { };
        public event Action BankAdvertWatchesCountChanged = delegate { };
        public event Action BankAdvertWatchTimeChanged = delegate { };

        private const string BankAdvertWatchesCountKey = "bank_advert_watches_count";
        private const string BankAdvertWatchTimeKey = "bank_advert_watch_time";

        private readonly Dictionary<AdvertTargetType, AdvertWatchState> _advertStates = new();
        
        private AdvertTargetType _lastPreparedTarget;
        private int _bankAdvertWatchesCount = -1;
        private int _bankAdvertWatchTime = -1;
        private UniTaskCompletionSource<bool> _currentShowingAdvertStateTcs;

        public UniTask<bool> CurrentShowingAdsTask => _currentShowingAdvertStateTcs?.Task ?? UniTask.FromResult(true);
        
        public int BankAdvertWatchesCount
        {
            get
            {
                if (_bankAdvertWatchesCount < 0)
                {
                    _bankAdvertWatchesCount = PlayerPrefs.GetInt(BankAdvertWatchesCountKey, 0);
                }
                return _bankAdvertWatchesCount;
            }
            set
            {
                _bankAdvertWatchesCount = value;
                PlayerPrefs.SetInt(BankAdvertWatchesCountKey, _bankAdvertWatchesCount);
                BankAdvertWatchesCountChanged();
            }
        }
        public int BankAdvertWatchTime
        {
            get
            {
                if (_bankAdvertWatchTime < 0)
                {
                    _bankAdvertWatchTime = PlayerPrefs.GetInt(BankAdvertWatchTimeKey, 0);
                }
                return _bankAdvertWatchTime;
            }
            set
            {
                _bankAdvertWatchTime = value;
                PlayerPrefs.SetInt(BankAdvertWatchTimeKey, _bankAdvertWatchTime);
                BankAdvertWatchTimeChanged();
            }
        }

        public void PrepareTarget(AdvertTargetType target)
        {
            _currentShowingAdvertStateTcs = new UniTaskCompletionSource<bool>();
            
            _advertStates[target] = AdvertWatchState.Prepared;
            _lastPreparedTarget = target;
            WatchStateChanged(_lastPreparedTarget);
        }

        public void MarkCurrentAsWatched()
        {
            _advertStates[_lastPreparedTarget] = AdvertWatchState.Watched;
            
            WatchStateChanged(_lastPreparedTarget);
            _currentShowingAdvertStateTcs.TrySetResult(true);
        }

        public void MarkCurrentAsCanceled()
        {
            _advertStates[_lastPreparedTarget] = AdvertWatchState.Default;
            
            WatchStateChanged(_lastPreparedTarget);
            _currentShowingAdvertStateTcs.TrySetResult(false);
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
        
        public bool IsDailyBonusAdsWatched()
        {
            return IsWatched(AdvertTargetType.DailyBonusRewardX2);
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

    public interface IBankAdvertWatchStateProvider
    {
        event Action BankAdvertWatchesCountChanged;
        event Action BankAdvertWatchTimeChanged;

        int BankAdvertWatchesCount { get; }
        int BankAdvertWatchTime { get; }
    }

    public enum AdvertTargetType
    {
        None,
        OfflineProfitX2,
        OfflineExpX2,
        BankGold,
        BankCash,
        DailyDouble1,
        DailyDouble2,
        DailyDouble3,
        DailyDouble4,
        DailyDouble5,
        DailyBonusRewardX2,
        DailyMissionRewardX2,
    }

    public enum AdvertWatchState
    {
        Default,
        Prepared,
        Watched,
    }
}