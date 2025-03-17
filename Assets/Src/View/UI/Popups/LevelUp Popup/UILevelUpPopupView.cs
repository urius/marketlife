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

        public void SetShareButtonVisibility(bool isVisible)
        {
            Buttons[1].gameObject.SetActive(isVisible);

            var rectTransform = (RectTransform)Buttons[0].transform;
            rectTransform.anchorMin = new Vector2(isVisible ? 0.25f : 0.5f, rectTransform.anchorMin.y);
            rectTransform.anchorMax = new Vector2(isVisible ? 0.25f : 0.5f, rectTransform.anchorMax.y);
        }
    }
}
