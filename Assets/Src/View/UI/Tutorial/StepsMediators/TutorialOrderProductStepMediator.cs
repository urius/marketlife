using UnityEngine;

public class TutorialOrderProductStepMediator : TutorialStepMediatorBase
{
    private readonly Dispatcher _dispatcher;
    private readonly TutorialUIElementsProvider _tutorialUIElementsProvider;
    private readonly GameStateModel _gameStateModel;

    //
    private RectTransform _itemSlotRect;

    public TutorialOrderProductStepMediator(RectTransform parentTransform)
        : base(parentTransform)
    {
        _dispatcher = Dispatcher.Instance;
        _tutorialUIElementsProvider = TutorialUIElementsProvider.Instance;
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
        _itemSlotRect = _tutorialUIElementsProvider.GetElementRectTransform(TutorialUIElement.OrderProductsPopupFirstItem);
        //Highlight element not on the pivot point
        var itemBoundsRect = _itemSlotRect.rect;
        var size = new Vector2(itemBoundsRect.size.x, itemBoundsRect.size.y * 1.5f);
        var corners = new Vector3[4];
        _itemSlotRect.GetWorldCorners(corners);
        View.HighlightScreenRoundArea((corners[0] + corners[2]) * 0.5f, size, animated: true);

        AllowClickOnRectTransform(_itemSlotRect);
    }
}
