public class DailyBonusPopupViewModel : PopupViewModelBase
{
    public readonly UserBonusState BonusState;
    public readonly IDailyBonusConfig BonusConfig;
    public readonly int OpenTimestamp;
    public readonly int CurrentBonusDay;

    public DailyBonusPopupViewModel(UserBonusState bonusState, IDailyBonusConfig bonusConfig, int openTimestamp)
    {
        BonusState = bonusState;
        BonusConfig = bonusConfig;
        OpenTimestamp = openTimestamp;
        CurrentBonusDay = DateTimeHelper.IsNextDay(bonusState.LastBonusTakeTimestamp, openTimestamp)
            ? bonusState.LastTakenBonusRank + 1
            : 1;
    }

    public override PopupType PopupType => PopupType.DailyBonus;
}
