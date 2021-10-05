using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOfflineReportPopupView : UITabbedContentPopupView
{
    public event Action ShareClicked = delegate { };

    [SerializeField] private Button _shareButton;
    [SerializeField] private TMP_Text _shareButtonText;
    [SerializeField] private TMP_Text _shareRevenueAmountText;

    public Transform ShareButtonTransform => _shareButton.transform;

    public override void Awake()
    {
        base.Awake();

        _shareButton.AddOnClickListener(OnShareClicked);
    }

    public void SetShareButtonText(string text)
    {
        _shareButtonText.text = text;
    }

    public void SetShareRevenueButtonText(string text)
    {
        _shareRevenueAmountText.text = text;
    }

    public void SetShareButtonInteractable(bool interactable)
    {
        _shareButton.interactable = interactable;
    }

    private void OnShareClicked()
    {
        ShareClicked();
    }
}
