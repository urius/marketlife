using UnityEngine;

public class PopupsMediator : IMediator
{
    private readonly RectTransform _contentTransform;
    private readonly GameStateModel _gameStateModel;

    private IMediator _currentPopupMediator;

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
        switch (_gameStateModel.ShowingPopupModel.PopupType)
        {
            case PopupType.ConfirmRemoveObject:
                _currentPopupMediator = new UIConfirmRemoveObjectPopupMediator(_contentTransform);
                break;
        };
        _currentPopupMediator.Mediate();
    }

    private void OnPopupRemoved()
    {
        if (_currentPopupMediator != null)
        {
            _currentPopupMediator.Unmediate();
            _currentPopupMediator = null;
        }
    }
}
