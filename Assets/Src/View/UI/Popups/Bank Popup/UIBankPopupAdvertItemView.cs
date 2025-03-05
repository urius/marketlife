using Src.View.UI.Common;
using UnityEngine;

namespace Src.View.UI.Popups.Bank_Popup
{
    public class UIBankPopupAdvertItemView : UIBankPopupItemViewBase
    {
        [SerializeField] private UINotificationCounterView _notificationCiunter;
        public UINotificationCounterView NotificationCounter => _notificationCiunter;

        public override void SetAvailable(bool isAvailable)
        {
            base.SetAvailable(isAvailable);
            _notificationCiunter.gameObject.SetActive(isAvailable);
        }
    }
}
