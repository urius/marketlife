using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBottomPanelFriendsTabView : MonoBehaviour
{
    [SerializeField] private TMP_Text _userNameText;
    [SerializeField] private Image _userIconImage;
    [SerializeField] private TMP_Text _userLevelText;
    [SerializeField] private TMP_Text _userExpText;
    [SerializeField] private TMP_Text _userCashText;
    [SerializeField] private UIBottomPanelFriendTabActionButtonView _takeActionView;
    [SerializeField] private UIBottomPanelFriendTabActionButtonView _unwashActionView;

    public UIBottomPanelFriendTabActionButtonView TakeActionView => _takeActionView;
    public UIBottomPanelFriendTabActionButtonView UnwashActionView => _unwashActionView;
    public RectTransform IconImageTransform => _userIconImage.rectTransform;

    public void SetNameText(string text)
    {
        _userNameText.text = text;
    }

    public void SetIconSprite(Sprite sprite)
    {
        _userIconImage.sprite = sprite;
    }

    public void SetLevelText(string text)
    {
        _userLevelText.text = text;
    }

    public void SetExpText(string text)
    {
        _userExpText.text = text;
    }

    public void SetCashText(string text)
    {
        _userCashText.text = text;
    }
}
