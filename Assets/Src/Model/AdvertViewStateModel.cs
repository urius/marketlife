using System;

public class AdvertViewStateModel
{
    public static readonly AdvertViewStateModel Instance = new AdvertViewStateModel();

    public event Action RewardStateChanged = delegate { };
    public event Action RewardStateReset = delegate { };

    public bool IsRewardCharged => RewardState == AdvertRewardState.Charged;

    public Price Reward { get; private set; }
    public AdvertRewardState RewardState { get; private set; }

    public void PrepareReward(Price reward)
    {
        Reward = reward;
        RewardState = AdvertRewardState.Prepared;
        RewardStateChanged();
    }

    public void ChargeReward()
    {
        RewardState = AdvertRewardState.Charged;
        RewardStateChanged();
    }

    public void ResetChargedReward()
    {
        Reward = new Price();
        RewardState = AdvertRewardState.Default;
        RewardStateReset();
    }
}

public enum AdvertRewardState
{
    Default,
    Prepared,
    Charged,
}
