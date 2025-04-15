using System;
using Src.Managers;
using UnityEngine;

public class TutorialDeliveringStepMediator : TutorialStepMediatorBase
{
    private readonly TutorialUIElementsProvider _tutorialUIElementProvider;
    private readonly ShopModel _playerShopModel;
    private readonly GameStateModel _gameStateModel;
    private readonly UpdatesProvider _updatesProvider;
    private readonly LocalizationManager _loc;

    private Action _realtimeSecondUpdateDelegate;
    private Action _realtimeUpdateDelegate;
    private int _minDeliverTime;
    private RectTransform _highlightRectTransform;

    public TutorialDeliveringStepMediator(RectTransform parentTransform)
        : base(parentTransform)
    {
        _tutorialUIElementProvider = TutorialUIElementsProvider.Instance;
        _playerShopModel = PlayerModelHolder.Instance.ShopModel;
        _gameStateModel = GameStateModel.Instance;
        _updatesProvider = UpdatesProvider.Instance;
        _loc = LocalizationManager.Instance;
    }

    public override void Mediate()
    {
        base.Mediate();

        _minDeliverTime = GetMinDeliverTime();
        View.DisableButton();
        _realtimeSecondUpdateDelegate = WaitForHightlightDeliver;

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
        _updatesProvider.RealtimeSecondUpdate += OnRealtimeSecondUpdate;
        _gameStateModel.ActionStateChanged += OnActionStateChanged;
    }

    private void Deactivate()
    {
        _updatesProvider.RealtimeUpdate -= OnRealtimeUpdate;
        _updatesProvider.RealtimeSecondUpdate -= OnRealtimeSecondUpdate;
        _gameStateModel.ActionStateChanged -= OnActionStateChanged;
    }

    private void OnActionStateChanged(ActionStateName prev, ActionStateName current)
    {
        if (current == ActionStateName.PlacingProductPlayer)
        {
            DispatchTutorialActionPerformed();
        }
    }

    private void WaitForHightlightDeliver()
    {
        if (_tutorialUIElementProvider.HasElement(TutorialUIElement.BottomPanelWarehouseTabLastDeliveringSlotTime))
        {
            if (_highlightRectTransform == null)
            {
                _highlightRectTransform = _tutorialUIElementProvider.GetElementRectTransform(TutorialUIElement.BottomPanelWarehouseTabLastDeliveringSlotTime);
                View.HighlightScreenRoundArea(GetDeliverTimeHighlightWorldPosition(), _highlightRectTransform.rect.size * 1.5f, animated: true);

                _realtimeSecondUpdateDelegate = WaitForDeliver;
                _realtimeUpdateDelegate = UpdateDeliverHighlightPosition;
            }
        }
    }

    private void WaitForDeliver()
    {
        if (_gameStateModel.ServerTime >= _minDeliverTime)
        {
            _realtimeUpdateDelegate = null;
            _highlightRectTransform = _tutorialUIElementProvider.GetElementRectTransform(TutorialUIElement.BottomPanelWarehouseTabLastDeliveringSlot);

            View.SetMessageText(_loc.GetLocalization($"{LocalizationKeys.TutorialMessagePrefix}{ViewModel.StepIndex}_a"));

            var itemBoundsRect = _highlightRectTransform.rect;
            var size = new Vector2(itemBoundsRect.size.x, itemBoundsRect.size.y) * 1.1f;
            View.HighlightScreenRoundArea(_highlightRectTransform.position, size, animated: true);

            AllowClickOnRectTransform(_highlightRectTransform);

            _realtimeSecondUpdateDelegate = null;
        }
    }

    private void UpdateDeliverHighlightPosition()
    {
        View.SetHighlightPosition(GetDeliverTimeHighlightWorldPosition());
    }

    private Vector3 GetDeliverTimeHighlightWorldPosition()
    {
        var corners = new Vector3[4];
        _highlightRectTransform.GetWorldCorners(corners);
        return (corners[0] + corners[2]) * 0.5f;
    }

    private void OnRealtimeUpdate()
    {
        _realtimeUpdateDelegate?.Invoke();
    }

    private void OnRealtimeSecondUpdate()
    {
        _realtimeSecondUpdateDelegate?.Invoke();
    }

    private int GetMinDeliverTime()
    {
        var result = 0;
        foreach (var slot in _playerShopModel.WarehouseModel.Slots)
        {
            if (slot.HasProduct && slot.Product.DeliverTime > _gameStateModel.ServerTime)
            {
                if (result == 0 || slot.Product.DeliverTime < result)
                {
                    result = slot.Product.DeliverTime;
                }
            }
        }

        return result;
    }
}
