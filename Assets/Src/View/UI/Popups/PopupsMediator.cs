using System.Collections.Generic;
using Src.Model;
using Src.Model.Popups;
using Src.View.UI.Popups.Bank_Popup;
using Src.View.UI.Popups.BillboardPopup;
using Src.View.UI.Popups.CashDeskPopup;
using Src.View.UI.Popups.DailyBonus_Popup;
using Src.View.UI.Popups.DailyMIssionsPopup;
using Src.View.UI.Popups.Error_Popup;
using Src.View.UI.Popups.LeaderboardsPopup;
using Src.View.UI.Popups.LevelUp_Popup;
using Src.View.UI.Popups.OfflineReport_Popup;
using Src.View.UI.Popups.OldGameCompensationPopup;
using Src.View.UI.Popups.OrderProduct_Popup;
using Src.View.UI.Popups.ShelfContent_Popup;
using Src.View.UI.Popups.Upgrades_Popup;
using Src.View.UI.Popups.Warehouse_Popup;
using UnityEngine;

namespace Src.View.UI.Popups
{
    public class PopupsMediator : IMediator
    {
        private readonly RectTransform _contentTransform;
        private readonly GameStateModel _gameStateModel;

        private Stack<IMediator> _popupMediators = new Stack<IMediator>();//todo /Stack/ check if using new types adds a weight to result build

        public PopupsMediator(RectTransform contentTransform)
        {
            _contentTransform = contentTransform;

            _gameStateModel = GameStateModel.Instance;
        }

        public void Mediate()
        {
            _gameStateModel.PopupShown += OnPopupShown;
            _gameStateModel.PopupRemoved += OnPopupRemoved;
        }

        public void Unmediate()
        {
            _gameStateModel.PopupShown -= OnPopupShown;
            _gameStateModel.PopupRemoved -= OnPopupRemoved;
        }

        private void OnPopupShown()
        {
            IMediator newMediator = null;
            switch (_gameStateModel.ShowingPopupModel.PopupType)
            {
                case PopupType.Confirm:
                    newMediator = new UIConfirmPopupMediator(_contentTransform);
                    break;
                case PopupType.OrderProduct:
                    newMediator = new UIOrderProductsPopupMediator(_contentTransform);
                    break;
                case PopupType.ShelfContent:
                    newMediator = new UIShelfContentPopupMediator(_contentTransform);
                    break;
                case PopupType.WarehouseForShelf:
                case PopupType.Warehouse:
                    newMediator = new UIWarehousePopupMediator(_contentTransform);
                    break;
                case PopupType.Upgrades:
                    newMediator = new UIUpgradesPopupMediator(_contentTransform);
                    break;
                case PopupType.OfflineReport:
                    newMediator = new UIOfflineReportPopupMediator(_contentTransform);
                    break;
                case PopupType.LevelUp:
                    newMediator = new UILevelUpPopupMediator(_contentTransform);
                    break;
                case PopupType.Bank:
                    newMediator = new UIBankPopupMediator(_contentTransform);
                    break;
                case PopupType.Error:
                    newMediator = new UIErrorPopupMediator(_contentTransform);
                    break;
                case PopupType.OldGameCompensation:
                    newMediator = new UIOldGameCompensationPopupMediator(_contentTransform);
                    break;
                case PopupType.DailyBonus:
                    newMediator = new UIDailyBonusPopupMediator(_contentTransform);
                    break;
                case PopupType.Billboard:
                    newMediator = new UIBillboardPopupMediator(_contentTransform);
                    break;
                case PopupType.CashDesk:
                    newMediator = new UICashDeskPopupMediator(_contentTransform);
                    break;
                case PopupType.DailyMissions:
                    newMediator = new UIDailyMissionsPopupMediator(_contentTransform);
                    break;
                case PopupType.Leaderboards:
                    newMediator = new UILeaderboardsPopupMediator(_contentTransform);
                    break;
            };
            newMediator.Mediate();
            _popupMediators.Push(newMediator);
        }

        private void OnPopupRemoved(PopupViewModelBase popupViewModel)
        {
            if (_popupMediators.Count > 0)
            {
                _popupMediators.Pop().Unmediate();
            }
        }
    }
}
