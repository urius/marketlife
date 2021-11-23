using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBottomPanelFriendItemView : UIBottomPanelScrollItemViewBase
{
    public event Action<UIBottomPanelFriendItemView> BottomButtonClicked = delegate { };

    [SerializeField] private TMP_Text _topText;
    [SerializeField] private Image _mainIconImage;
    [SerializeField] private Image _statusIconImage;
    [SerializeField] private Image _bgImage;
    [SerializeField] private UIHintableView _mainHintableView;
    [SerializeField] private Button _bottomButton;
    [SerializeField] private Color _colorTextDefault;

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

    public void SetStatusIconImageSprite(Sprite sprite)
    {
        var isShown = sprite != null;
        _statusIconImage.gameObject.SetActive(isShown);
        if (isShown)
        {
            _statusIconImage.sprite = sprite;
        }
    }

    public void SetMainHintEnabled(bool isEnabled)
    {
        _mainHintableView.enabled = isEnabled;
    }

    public void SetMainHintText(string text)
    {
        _mainHintableView.DisplayText = text;
    }

    public void SetBottomButtonEnabled(bool isEnabled)
    {
        _bottomButton.gameObject.SetActive(isEnabled);
    }

    public void SetTextDefaultColor()
    {
        _topText.color = _colorTextDefault;
    }

    public void SetTextAltColor()
    {
        _topText.color = Color.white;
    }

    public void SetBgImageDefaultColor()
    {
        _bgImage.color = Color.white;
    }

    public void SetBgImageColor(Color color)
    {
        _bgImage.color = color;
    }

    private void OnBottomButtonClicked()
    {
        BottomButtonClicked(this);
    }
}
