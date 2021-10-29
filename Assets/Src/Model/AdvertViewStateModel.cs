using System;

public class AdvertViewStateModel
{
    public static readonly AdvertViewStateModel Instance = new AdvertViewStateModel();

    public event Action RewardCharged = delegate { };
    public event Action RewardChargeReset = delegate { };

    public bool IsRewardCharged { get; private set; } = false;
    public Price ChargedReward { get; private set; }

    public void ChargeReward(Price reward)
    {
        ChargedReward = reward;
        IsRewardCharged = true;
        RewardCharged();
    }

    public void ResetChargedReward()
    {
        ChargedReward = new Price();
        IsRewardCharged = false;
        RewardChargeReset();
    }
}
