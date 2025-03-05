using TMPro;
using UnityEngine;

namespace Src.View.UI.Popups.DailyMIssionsPopup
{
    public class UIDailyMissionsPopupView : UIContentPopupView
    {
        [SerializeField] private TMP_Text _statusText;

        public void SetStatusText(string text)
        {
            _statusText.text = text;
        }
    }
}
