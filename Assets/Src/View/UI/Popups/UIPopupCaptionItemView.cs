using TMPro;
using UnityEngine;

namespace Src.View.UI.Popups
{
    public class UIPopupCaptionItemView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}
