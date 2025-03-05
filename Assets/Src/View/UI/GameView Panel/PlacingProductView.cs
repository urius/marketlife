using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Src.View.UI.GameView_Panel
{
    public class PlacingProductView : MonoBehaviour
    {
        [SerializeField] private Image _productImage;
        [SerializeField] private TMP_Text _amountText;

        public void SetImageSprite(Sprite sprite)
        {
            _productImage.sprite = sprite;
        }

        public void SetText(string text)
        {
            _amountText.text = text;
        }
    }
}
