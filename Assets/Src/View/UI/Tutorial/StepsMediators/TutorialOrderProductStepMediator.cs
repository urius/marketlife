using System;
using UnityEngine;

public class TutorialOrderProductStepMediator : TutorialStepMediatorBase
{
    private readonly Dispatcher _dispatcher;
    private readonly TutorialUIElementsProvider _tutorialUIElementsProvider;
    private readonly ScreenCalculator _screenCalculator;
    private readonly UpdatesProvider _updatesProvider;
    private readonly GameStateModel _gameStateModel;

    //
    private Camera _camera;
    private RectTransform _itemSlotRect;

    public TutorialOrderProductStepMediator(RectTransform parentTransform)
        : base(parentTransform)
    {
        _dispatcher = Dispatcher.Instance;
        _tutorialUIElementsProvider = TutorialUIElementsProvider.Instance;
        _screenCalculator = ScreenCalculator.Instance;
        _updatesProvider = UpdatesProvider.Instance;
        _gameStateModel = GameStateModel.Instance;
    }

    public override void Mediate()
    {
        base.Mediate();

        _camera = Camera.main;

        View.DisableButton();
        View.SetQuadrant(3);

        Activate();
    }

    public override void Unmediate()
    {
        Deactivate();

        base.Unmediate();
    }

    private void Activate()
    {
        _dispatcher.UIOrderPopupAppeared += OnUIOrderPopupAppeared;
        _updatesProvider.RealtimeUpdate += OnRealtimeUpdate;
        _gameStateModel.PopupRemoved += OnPopupRemoved;
    }

    private void Deactivate()
    {
        _dispatcher.UIOrderPopupAppeared -= OnUIOrderPopupAppeared;
        _updatesProvider.RealtimeUpdate -= OnRealtimeUpdate;
        _gameStateModel.PopupRemoved -= OnPopupRemoved;
    }

    private void OnPopupRemoved()
    {
        _dispatcher.TutorialActionPerformed();
    }

    private void OnUIOrderPopupAppeared()
    {
        _itemSlotRect = _tutorialUIElementsProvider.GetElementRect(TutorialUIElement.OrderProductsPopupFirstItem);
        var itemBoundsRect = _itemSlotRect.rect;
        var size = new Vector2(itemBoundsRect.size.x, itemBoundsRect.size.y * 1.6f);
        var offset = new Vector3(itemBoundsRect.size.x * 0.5f, -itemBoundsRect.size.y * 0.5f);
        View.HighlightScreenPoint(_screenCalculator.WorldToScreenPoint(_itemSlotRect.position) + offset, size, animated: true);
    }

    private void OnRealtimeUpdate()
    {
        var isMouseOverRect = RectTransformUtility.RectangleContainsScreenPoint(_itemSlotRect, Input.mousePosition, _camera);
        View.SetClickBlockState(!isMouseOverRect);
    }
}
