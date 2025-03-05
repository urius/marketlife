using TMPro;
using UnityEngine;

namespace Src.View.UI.Popups.OfflineReport_Popup
{
    public class UIOfflineReportPopupOverallItemView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _leftText;
        [SerializeField] private TMP_Text _cashText;
        [SerializeField] private TMP_Text _expText;

        public void SetLeftText(string text)
        {
            _leftText.text = text;
        }

        public void SetCashText(string text)
        {
            _cashText.text = text;
        }

        public void SetExpText(string text)
        {
            _expText.text = text;
        }
    }
}
