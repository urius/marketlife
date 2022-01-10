public class DailyMissionsPopupViewModel : PopupViewModelBase
{
    public readonly DailyMissionsModel DailyMissionsModel;

    public DailyMissionsPopupViewModel(DailyMissionsModel dailyMissionsModel)
    {
        DailyMissionsModel = dailyMissionsModel;
    }

    public override PopupType PopupType => PopupType.DailyMissions;
}
