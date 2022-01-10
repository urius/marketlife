using UnityEngine;

[CreateAssetMenu(fileName = "ColorsHolder", menuName = "Scriptable Objects/ColorsHolder")]
public class ColorsHolder : ScriptableObject
{
    public static ColorsHolder Instance { get; private set; }

    [Space(10)]
    public Color BottomPanelFriendsTabAddButtonColor;
    public Color BottomPanelFriendsInactiveFriendButtonColor;
    [Space(10)]
    public Color ReportPopupItemRightTextProfitColor;
    public Color ReportPopupItemPositiveActionColor;
    public Color ReportPopupItemNegativeActionColor;
    [Space(10)]
    public Color DailyMissionNewMissionsNotificationColor;
    public Color DailyMissionCompletedMissionsNotificationColor;

    private void OnEnable()
    {
        Instance = this;
    }
}
