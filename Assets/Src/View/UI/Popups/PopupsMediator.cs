using System.Collections.Generic;
using UnityEngine;

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
        };
        newMediator.Mediate();
        _popupMediators.Push(newMediator);
    }

    private void OnPopupRemoved()
    {
        if (_popupMediators.Count > 0)
        {
            _popupMediators.Pop().Unmediate();
        }
    }
}
