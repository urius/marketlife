using System;
using UnityEngine;
using UnityEngine.UI;

public class UIDailyBonusPopupView : UIPopupBase
{
    public event Action TakeButtonClicked = delegate { };

    [SerializeField] private Button _takeButton;
    [SerializeField] private UIDailyBonusPopupPrizeItemView[] _itemViews;

    public UIDailyBonusPopupPrizeItemView[] ItemViews => _itemViews;

    public override void Awake()
    {
        base.Awake();

        _takeButton.AddOnClickListener(OnTakeClicked);
    }

    public void SetTakeButtonInteractable(bool isInteractable)
    {
        _takeButton.interactable = isInteractable;
    }

    private void OnTakeClicked()
    {
        TakeButtonClicked();
    }
}
