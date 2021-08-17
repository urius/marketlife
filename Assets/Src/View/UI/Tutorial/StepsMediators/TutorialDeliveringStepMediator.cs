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
        _updatesProvider.RealtimeUpdate += OnRealtimeUpdate;
        _updatesProvider.RealtimeSecondUpdate += OnRealtimeSecondUpdate;
        _gameStateModel.PlacingStateChanged += OnPlacingStateChanged;
    }

    private void Deactivate()
    {
        _updatesProvider.RealtimeUpdate -= OnRealtimeUpdate;
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
        if (_tutorialUIElementProvider.HasElement(TutorialUIElement.BottomPanelWarehouseTabLastDeliveringSlotTime))
        {
            if (_highlightRectTransform == null)
            {
                _highlightRectTransform = _tutorialUIElementProvider.GetElementRectTransform(TutorialUIElement.BottomPanelWarehouseTabLastDeliveringSlotTime);
                View.HighlightScreenRoundArea(GetDeliverTimeHighlightPosition(), _highlightRectTransform.rect.size * 1.5f, animated: true);

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
            View.HighlightScreenRoundArea(_screenCalculator.WorldToScreenPoint(_highlightRectTransform.position), size, animated: true);

            AllowClickOnRectTransform(_highlightRectTransform);

            _realtimeSecondUpdateDelegate = null;
        }
    }

    private void UpdateDeliverHighlightPosition()
    {
        View.SetHighlightPosition(GetDeliverTimeHighlightPosition());
    }

    private Vector3 GetDeliverTimeHighlightPosition()
    {
        return _screenCalculator.WorldToScreenPoint(_highlightRectTransform.position) + new Vector3(0, _highlightRectTransform.rect.size.y * 0.5f, 0);
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
