using System;
using UnityEngine;

public class TutorialDeliveringStepMediator : TutorialStepMediatorBase
{
    private readonly TutorialUIElementsProvider _tutorialUIElementProvider;
    private readonly ShopModel _playerShopModel;
    private readonly GameStateModel _gameStateModel;
    private readonly UpdatesProvider _updatesProvider;
    private readonly ScreenCalculator _screenCalculator;
    private readonly LocalizationManager _loc;

    private Action _realtimeSecondUpdateDelegate;
    private int _minDeliverTime;
    private RectTransform _deliveringRectTransform;

    public TutorialDeliveringStepMediator(RectTransform parentTransform)
        : base(parentTransform)
    {
        _tutorialUIElementProvider = TutorialUIElementsProvider.Instance;
        _playerShopModel = PlayerModelHolder.Instance.ShopModel;
        _gameStateModel = GameStateModel.Instance;
        _updatesProvider = UpdatesProvider.Instance;
        _screenCalculator = ScreenCalculator.Instance;
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
        _updatesProvider.RealtimeSecondUpdate += OnRealtimeSecondUpdate;
        _gameStateModel.PlacingStateChanged += OnPlacingStateChanged;
    }

    private void Deactivate()
    {
        _updatesProvider.RealtimeSecondUpdate -= OnRealtimeSecondUpdate;
        _gameStateModel.PlacingStateChanged -= OnPlacingStateChanged;
    }

    private void OnPlacingStateChanged(PlacingStateName prev, PlacingStateName current)
    {
        if (current == PlacingStateName.PlacingProduct)
        {
            DispatchTutorialActionPerformed();
        }
    }

    private void WaitForHightlightDeliver()
    {
        if (_tutorialUIElementProvider.HasElement(TutorialUIElement.BottomPanelWarehouseTabLastDeliveringSlot))
        {
            if (_deliveringRectTransform == null)
            {
                _deliveringRectTransform = _tutorialUIElementProvider.GetElementRectTransform(TutorialUIElement.BottomPanelWarehouseTabLastDeliveringSlot);
                var itemBoundsRect = _deliveringRectTransform.rect;
                var size = new Vector2(itemBoundsRect.size.x, itemBoundsRect.size.y * 0.4f);
                var offset = new Vector3(0, itemBoundsRect.size.y * 0.5f);
                View.HighlightScreenRoundArea(_screenCalculator.WorldToScreenPoint(_deliveringRectTransform.position) + offset, size, animated: true);
                _realtimeSecondUpdateDelegate = WaitForDeliver;
            }
        }
    }

    private void WaitForDeliver()
    {
        if (_gameStateModel.ServerTime >= _minDeliverTime)
        {
            View.SetMessageText(_loc.GetLocalization($"{LocalizationKeys.TutorialMessagePrefix}{ViewModel.StepIndex}_a"));

            var itemBoundsRect = _deliveringRectTransform.rect;
            var size = new Vector2(itemBoundsRect.size.x, itemBoundsRect.size.y) * 1.1f;
            View.HighlightScreenRoundArea(_screenCalculator.WorldToScreenPoint(_deliveringRectTransform.position), size, animated: true);

            AllowClickOnRectTransform(_deliveringRectTransform);

            _realtimeSecondUpdateDelegate = null;
        }
        else
        {
            var itemBoundsRect = _deliveringRectTransform.rect;
            var offset = new Vector3(0, itemBoundsRect.size.y * 0.5f);
            View.SetHighlightPosition(_screenCalculator.WorldToScreenPoint(_deliveringRectTransform.position) + offset);
        }
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
