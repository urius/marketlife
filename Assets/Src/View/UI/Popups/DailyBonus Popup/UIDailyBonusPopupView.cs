using System;
using Src.Common_Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Src.View.UI.Popups.DailyBonus_Popup
{
    public class UIDailyBonusPopupView : UIPopupBase
    {
        public event Action TakeButtonClicked;
        public event Action TakeButtonX2Clicked;

        [SerializeField] private UIDailyBonusPopupPrizeItemView[] _itemViews;
        [SerializeField] private Button _takeButton;
        [SerializeField] private Button _takeX2Button;

        public UIDailyBonusPopupPrizeItemView[] ItemViews => _itemViews;

        public override void Awake()
        {
            base.Awake();
            
            _takeButton.AddOnClickListener(OnTakeButtonClicked);
            _takeX2Button.AddOnClickListener(OnTakeX2ButtonClicked);
        }

        public void SetTakeButtonsInteractable(bool isInteractable)
        {
            _takeButton.interactable = isInteractable;
            _takeX2Button.interactable = isInteractable;
        }

        private void OnTakeButtonClicked()
        {
            TakeButtonClicked?.Invoke();
        }

        private void OnTakeX2ButtonClicked()
        {
            TakeButtonX2Clicked?.Invoke();
        }
    }
}
