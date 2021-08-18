using System;
using UnityEngine;

public class TutorialOpenWarehouseStepMediator : TutorialStepMediatorBase
{
    private readonly TutorialUIElementsProvider _tutorialUIElementsProvider;
    private readonly BottomPanelViewModel _bottomPanelViewModel;

    private RectTransform _warehouseButtonRect;

    public TutorialOpenWarehouseStepMediator(RectTransform parentTransform)
         : base(parentTransform)
    {
        _tutorialUIElementsProvider = TutorialUIElementsProvider.Instance;
        _bottomPanelViewModel = GameStateModel.Instance.BottomPanelViewModel;        
    }

    public override void Mediate()
    {
        base.Mediate();

        View.DisableButton();
        _warehouseButtonRect = _tutorialUIElementsProvider.GetElementRectTransform(TutorialUIElement.BottomPanelWarehouseButton);
        var sideSize = Math.Max(_warehouseButtonRect.rect.size.x, _warehouseButtonRect.rect.size.y);
        View.HighlightScreenRoundArea(_warehouseButtonRect.position, new Vector2(sideSize, sideSize), animated: true);
        AllowClickOnRectTransform(_warehouseButtonRect);

        Activate();
    }

    public override void Unmediate()
    {
        Deactivate();

        base.Unmediate();
    }

    private void Activate()
    {
        _bottomPanelViewModel.SimulationTabChanged += OnSimulationTabChanged;
    }

    private void Deactivate()
    {
        _bottomPanelViewModel.SimulationTabChanged -= OnSimulationTabChanged;
    }

    private void OnSimulationTabChanged()
    {
        if (_bottomPanelViewModel.SimulationModeTab == BottomPanelSimulationModeTab.Warehouse)
        {
            DispatchTutorialActionPerformed();
        }
    }
}
