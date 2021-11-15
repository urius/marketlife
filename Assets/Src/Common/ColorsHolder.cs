using UnityEngine;

[CreateAssetMenu(fileName = "ColorsHolder", menuName = "Scriptable Objects/ColorsHolder")]
public class ColorsHolder : ScriptableObject
{
    public static ColorsHolder Instance { get; private set; }

    [Space(10)]
    public Color BottomPanelFriendsTabAddButtonColor;
    public Color BottomPanelFriendsInactiveFriendButtonColor;

    private void OnEnable()
    {
        Instance = this;
    }
}
