using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUpgradesPopupMediator : IMediator
{
    private readonly RectTransform _parentTransform;
    private readonly GameStateModel _gameStateModel;

    private UpgradesPopupViewModel _model;

    public UIUpgradesPopupMediator(RectTransform parentTransform)
    {
        _parentTransform = parentTransform;
        _gameStateModel = GameStateModel.Instance;
    }

    public void Mediate()
    {
        _model = _gameStateModel.ShowingPopupModel as UpgradesPopupViewModel;
    }

    public void Unmediate()
    {
    }
}
