using Src.Common_Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Src.View.UI.Common
{
    public class UISaveIconView : MonoBehaviour
    {
        [SerializeField] private RectTransform _imageRectTransform;
        public RectTransform ImageRectTransform => _imageRectTransform;
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _text;

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void SetAlpha(float alpha)
        {
            _image.color.SetAlpha(alpha);
            _text.color.SetAlpha(alpha);
        }
    }
}
