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

    private void OnEnable()
    {
        Instance = this;
    }
}
