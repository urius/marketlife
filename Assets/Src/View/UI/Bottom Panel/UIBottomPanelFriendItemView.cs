using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBottomPanelFriendItemView : MonoBehaviour
{
    public event Action<UIBottomPanelFriendItemView> MainButtonClicked = delegate { };
    public event Action<UIBottomPanelFriendItemView> BottomButtonClicked = delegate { };

    [SerializeField] private TMP_Text _topText;
    [SerializeField] private Button _mainButton;
    [SerializeField] private Image _mainIconImage;
    [SerializeField] private UIHintableView _mainHintableView;
    [SerializeField] private Button _bottomButton;

    public void Awake()
    {
        _mainButton.AddOnClickListener(OnMainButtonClicked);
        _bottomButton.AddOnClickListener(OnBottomButtonClicked);
    }

    public void SetTopText(string text)
    {
        _topText.text = text;
    }

    public void SetMainIconImageSprite(Sprite sprite)
    {
        _mainIconImage.sprite = sprite;
    }

    public void SetMainHintEnabled(bool isEnabled)
    {
        _mainHintableView.enabled = isEnabled;
    }

    public void SetMainHintText(string text)
    {
        _mainHintableView.DisplayText = text;
    }

    private void OnMainButtonClicked()
    {
        MainButtonClicked(this);
    }

    private void OnBottomButtonClicked()
    {
        BottomButtonClicked(this);
    }
}
