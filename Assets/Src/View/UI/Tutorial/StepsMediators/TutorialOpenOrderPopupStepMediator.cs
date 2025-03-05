using System;
using Src.Common;
using Src.Model;
using Src.Model.Popups;
using UnityEngine;

namespace Src.View.UI.Tutorial.StepsMediators
{
    public class TutorialOpenOrderPopupStepMediator : TutorialStepMediatorBase
    {
        private readonly TutorialUIElementsProvider _tutorialUIElementsProvider;
        private readonly UpdatesProvider _updatesProvider;
        private readonly GameStateModel _gameStateModel;

        private RectTransform _emptySlotRect;
        private Vector3 _higlightedSlotSavedPosition;

        public TutorialOpenOrderPopupStepMediator(RectTransform parentTransform)
            : base(parentTransform)
        {
            _tutorialUIElementsProvider = TutorialUIElementsProvider.Instance;
            _updatesProvider = UpdatesProvider.Instance;
            _gameStateModel = GameStateModel.Instance;
        }

        public override void Mediate()
        {
            base.Mediate();
            View.DisableButton();
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
            _gameStateModel.PopupShown += OnPopupShown;
        }

        private void Deactivate()
        {
            _updatesProvider.RealtimeUpdate -= OnRealtimeUpdate;
            _gameStateModel.PopupShown -= OnPopupShown;
        }

        private void OnPopupShown()
        {
            if (_gameStateModel.ShowingPopupModel.PopupType == PopupType.OrderProduct)
            {
                DispatchTutorialActionPerformed();
            }
        }

        private void OnRealtimeUpdate()
        {
            if (_emptySlotRect == null)
            {
                _emptySlotRect = _tutorialUIElementsProvider.GetElementRectTransform(TutorialUIElement.BottomPanelWarehouseTabFirstFreeSlot);
                if (_emptySlotRect != null)
                {
                    var sideSize = Math.Max(_emptySlotRect.rect.size.x, _emptySlotRect.rect.size.y) * 1.1f;
                    _higlightedSlotSavedPosition = _emptySlotRect.position;
                    View.HighlightScreenRoundArea(_emptySlotRect.position, new Vector2(sideSize, sideSize), animated: true);
                    AllowClickOnRectTransform(_emptySlotRect);
                }
            }
            else if (_higlightedSlotSavedPosition != _emptySlotRect.position)
            {
                _higlightedSlotSavedPosition = _emptySlotRect.position;
                View.SetHighlightPosition(_higlightedSlotSavedPosition);
            }
        }
    }
}
