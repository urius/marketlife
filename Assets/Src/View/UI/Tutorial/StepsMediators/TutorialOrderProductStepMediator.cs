using UnityEngine;

public class TutorialOrderProductStepMediator : TutorialStepMediatorBase
{
    private readonly Dispatcher _dispatcher;
    private readonly TutorialUIElementsProvider _tutorialUIElementsProvider;
    private readonly ScreenCalculator _screenCalculator;
    private readonly GameStateModel _gameStateModel;

    //
    private RectTransform _itemSlotRect;

    public TutorialOrderProductStepMediator(RectTransform parentTransform)
        : base(parentTransform)
    {
        _dispatcher = Dispatcher.Instance;
        _tutorialUIElementsProvider = TutorialUIElementsProvider.Instance;
        _screenCalculator = ScreenCalculator.Instance;
        _gameStateModel = GameStateModel.Instance;
    }

    public override void Mediate()
    {
        base.Mediate();

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
        _gameStateModel.PopupRemoved += OnPopupRemoved;
    }

    private void Deactivate()
    {
        _dispatcher.UIOrderPopupAppeared -= OnUIOrderPopupAppeared;
        _gameStateModel.PopupRemoved -= OnPopupRemoved;
    }

    private void OnPopupRemoved()
    {
        DispatchTutorialActionPerformed();
    }

    private void OnUIOrderPopupAppeared()
    {
        //return;
        _itemSlotRect = _tutorialUIElementsProvider.GetElementRectTransform(TutorialUIElement.OrderProductsPopupFirstItem);
        var itemBoundsRect = _itemSlotRect.rect;
        var size = new Vector2(itemBoundsRect.size.x, itemBoundsRect.size.y * 1.6f);
        var offset = new Vector3(itemBoundsRect.size.x * 0.5f, -itemBoundsRect.size.y * 0.5f);
        View.HighlightScreenRoundArea(_screenCalculator.WorldToScreenPoint(_itemSlotRect.position) + offset, size, animated: true);

        AllowClickOnRectTransform(_itemSlotRect);
    }
}
