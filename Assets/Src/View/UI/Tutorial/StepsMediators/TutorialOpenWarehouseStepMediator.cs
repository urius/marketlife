using System;
using UnityEngine;

public class TutorialOpenWarehouseStepMediator : TutorialStepMediatorBase
{
    private readonly TutorialUIElementsProvider _tutorialUIElementsProvider;
    private readonly ScreenCalculator _screenCalculator;
    private readonly UpdatesProvider _updatesProvider;
    private readonly Dispatcher _dispatcher;

    private Camera _camera;
    private RectTransform _warehouseButtonRect;

    public TutorialOpenWarehouseStepMediator(RectTransform parentTransform)
         : base(parentTransform)
    {
        _tutorialUIElementsProvider = TutorialUIElementsProvider.Instance;
        _screenCalculator = ScreenCalculator.Instance;
        _updatesProvider = UpdatesProvider.Instance;
        _dispatcher = Dispatcher.Instance;
    }

    public override void Mediate()
    {
        base.Mediate();

        _camera = Camera.main;

        View.DisableButton();

        _warehouseButtonRect = _tutorialUIElementsProvider.GetElementRect(TutorialUIElement.BottomPanelWarehouseButton);
        var sideSize = Math.Max(_warehouseButtonRect.rect.size.x, _warehouseButtonRect.rect.size.y);
        View.HighlightScreenPoint(_screenCalculator.WorldToScreenPoint(_warehouseButtonRect.position), new Vector2(sideSize, sideSize), animated: true);

        Activate();
    }

    public override void Unmediate()
    {
        Deactivate();

        base.Unmediate();
    }

    private void Activate()
    {
        _updatesProvider.RealtimeUpdate += OnRealtimeUpdate;
        _dispatcher.BottomPanelWarehouseTabOpened += OnWarehouseTabOpened;
    }

    private void Deactivate()
    {
        _updatesProvider.RealtimeUpdate -= OnRealtimeUpdate;
        _dispatcher.BottomPanelWarehouseTabOpened -= OnWarehouseTabOpened;
    }

    private void OnRealtimeUpdate()
    {
        var isMouseOverRect = RectTransformUtility.RectangleContainsScreenPoint(_warehouseButtonRect, Input.mousePosition, _camera);
        View.SetClickBlockState(!isMouseOverRect);
    }

    private void OnWarehouseTabOpened()
    {
        _dispatcher.TutorialActionPerformed();
    }
}
