using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BottomPanelMediator : UINotMonoMediatorBase
{
    private readonly BottomPanelView _view;

    private UINotMonoMediatorBase _currentTabMediator;

    public BottomPanelMediator(BottomPanelView view)
        : base(view.transform as RectTransform)
    {
        _view = view;
    }

    public override async void Mediate()
    {
        base.Mediate();

        _currentTabMediator = new UIBottomPanelShelfsTabMediator(_view);
        _currentTabMediator.Mediate();

        Activate();

        await _view.ShowInteriorModeButtonsAsync();
    }

    private void Activate()
    {
        _view.InteriorButtonClicked += OnInteriorButtonClicked;
        _view.InteriorCloseButtonClicked += OnInteriorCloseButtonClicked;
    }

    private async void OnInteriorButtonClicked()
    {
        await _view.HideSimulationModeButtonsAsync();
        await _view.ShowInteriorModeButtonsAsync();
    }

    private async void OnInteriorCloseButtonClicked()
    {
        await _view.HideInteriorModeButtonsAsync();
        await _view.ShowSimulationModeButtonsAsync();
    }

    public override void Unmediate()
    {
        base.Unmediate();
    }
}
