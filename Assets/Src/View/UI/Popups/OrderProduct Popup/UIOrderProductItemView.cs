using System;
using Src.Common_Utils;
using Src.View.UI.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Src.View.UI.Popups.OrderProduct_Popup
{
    public class UIOrderProductItemView : MonoBehaviour
    {
        public event Action<UIOrderProductItemView> Cliked = delegate { };

        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Image _productIcon;
        [SerializeField] private UIPriceLabelView _priceLabelView;
        [SerializeField] private TMP_Text _demandText;
        [SerializeField] private UIProgressColorLineView _demandPercentView;

        public void Awake()
        {
            _button.AddOnClickListener(OnButtonClick);
        }

        public Vector3 GetIconPosition()
        {
            return _productIcon.rectTransform.position;
        }

        public void SetTitleText(string text)
        {
            _titleText.text = text;
        }
    
        public void SetDemandText(string text)
        {
            _demandText.text = text;
        }
    
        public void SetDemandPercent(float value)
        {
            _demandPercentView.SetProgress(value);
        }

        public void SetDescriptionText(string text)
        {
            _descriptionText.text = text;
        }

        public void SetProductIcon(Sprite sprite)
        {
            _productIcon.sprite = sprite;
        }

        public void SetPrice(bool isGold, int amount)
        {
            _priceLabelView.SetType(isGold);
            _priceLabelView.Value = amount;
        }

        private void OnButtonClick()
        {
            Cliked(this);
        }
    }
}
