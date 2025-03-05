using TMPro;
using UnityEngine;

namespace Src.View.UI.Popups.LevelUp_Popup
{
    public class UILevelUpPopupView : UIContentPopupWithDescriptionAndButtons
    {
        [SerializeField] private TMP_Text _shareRevenueAmountText;

        public Transform ShareButtonTransform => Buttons[1].transform;

        public void SetShareRevenueButtonText(string text)
        {
            _shareRevenueAmountText.text = text;
        }

        public void SetShareButtonInteractable(bool interactable)
        {
            Buttons[1].interactable = interactable;
        }
    }
}
