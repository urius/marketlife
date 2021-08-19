using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBottomPanelFriendItemView : UIBottomPanelScrollItemViewBase
{
    public event Action<UIBottomPanelFriendItemView> MainButtonClicked = delegate { };
    public event Action<UIBottomPanelFriendItemView> BottomButtonClicked = delegate { };

    [SerializeField] private TMP_Text _topText;
    [SerializeField] private Image _mainIconImage;
    [SerializeField] private UIHintableView _mainHintableView;
    [SerializeField] private Button _bottomButton;

    public override void Awake()
    {
        base.Awake();
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

    private void OnBottomButtonClicked()
    {
        BottomButtonClicked(this);
    }
}
