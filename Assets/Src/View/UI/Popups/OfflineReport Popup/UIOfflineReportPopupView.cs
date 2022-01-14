using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOfflineReportPopupView : UITabbedContentPopupView
{
    public event Action ShareClicked = delegate { };
    public event Action AdsClicked = delegate { };

    [Space()]
    [SerializeField] private Button _adsButton;
    [SerializeField] private TMP_Text _adsButtonText;
    [SerializeField] private Button _shareButton;
    [SerializeField] private TMP_Text _shareButtonText;
    [SerializeField] private TMP_Text _shareRevenueAmountText;

    public Transform ShareButtonTransform => _shareButton.transform;

    public override void Awake()
    {
        base.Awake();

        _shareButton.AddOnClickListener(OnShareClicked);
        _adsButton.AddOnClickListener(OnAdsClicked);
    }

    public void SetShareButtonText(string text)
    {
        _shareButtonText.text = text;
    }

    public void SetShareRevenueButtonText(string text)
    {
        _shareRevenueAmountText.text = text;
    }

    public void SetShareButtonVisiblity(bool isVisible)
    {
        _shareButton.gameObject.SetActive(isVisible);
    }

    public void SetShareButtonInteractable(bool interactable)
    {
        _shareButton.interactable = interactable;
    }

    public void SetAdsButtonInteractable(bool interactable)
    {
        _adsButton.interactable = interactable;
    }

    public void SetAdsButtonText(string text)
    {
        _adsButtonText.text = text;
    }

    public void SetAdsButtonSkinSprite(Sprite sprite)
    {
        _adsButton.image.sprite = sprite;
    }

    public void SetAdsButtonVisibility(bool isVisible)
    {
        _adsButton.gameObject.SetActive(isVisible);
    }

    private void OnShareClicked()
    {
        ShareClicked();
    }

    private void OnAdsClicked()
    {
        AdsClicked();
    }
}
