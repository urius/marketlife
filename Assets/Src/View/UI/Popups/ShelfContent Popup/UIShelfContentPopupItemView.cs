using System;
using Src.Common_Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Src.View.UI.Popups.ShelfContent_Popup
{
    public class UIShelfContentPopupItemView : MonoBehaviour
    {
        public event Action<UIShelfContentPopupItemView> Cliked = delegate { };
        public event Action<UIShelfContentPopupItemView> RemoveCliked = delegate { };

        [SerializeField] private Button _mainButton;
        [SerializeField] private Button _removeButton;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Image _productIcon;

        public void Awake()
        {
            _mainButton.AddOnClickListener(OnMainButtonClick);
            _removeButton.AddOnClickListener(OnRemoveButtonClick);
        }

        public void SetMainButtonInteractable(bool isInteractable)
        {
            _mainButton.interactable = isInteractable;
        }

        public void SetRemoveButtonVisibility(bool isVisible)
        {
            _removeButton.gameObject.SetActive(isVisible);
        }

        public void SetTitleText(string text)
        {
            _titleText.text = text;
        }

        public void SetDescriptionText(string text)
        {
            _descriptionText.text = text;
        }

        public void SetProductIcon(Sprite sprite)
        {
            _productIcon.gameObject.SetActive(sprite != null);
            _productIcon.sprite = sprite;
        }

        private void OnMainButtonClick()
        {
            Cliked(this);
        }

        private void OnRemoveButtonClick()
        {
            RemoveCliked(this);
        }
    }
}
