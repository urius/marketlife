using System.Collections.Generic;
using Src.Common;
using Src.Managers;
using Src.View.Gameplay;
using UnityEngine;

namespace Src.View.UI.GameView_Panel
{
    public class UIFlyingTextsMediator : IMediator
    {
        private readonly RectTransform _contentTransform;
        private readonly Dispatcher _dispatcher;
        private readonly GameConfigManager _configManager;
        private readonly ScreenCalculator _screenCalculator;
        private readonly PrefabsHolder _prefabsHolder;
        private readonly SpritesProvider _spritesProvider;
        private readonly GridCalculator _gridCalculator;
        private readonly List<(RectTransform transform, Vector3 worldPosition)> _itemsWithOffsetsList = new List<(RectTransform, Vector3)>(5);

        private Camera _camera;

        public UIFlyingTextsMediator(RectTransform contentTransform)
        {
            _contentTransform = contentTransform;

            _dispatcher = Dispatcher.Instance;
            _configManager = GameConfigManager.Instance;
            _screenCalculator = ScreenCalculator.Instance;
            _prefabsHolder = PrefabsHolder.Instance;
            _spritesProvider = SpritesProvider.Instance;
            _gridCalculator = GridCalculator.Instance;
        }

        public void Mediate()
        {
            _camera = Camera.main;

            _dispatcher.UIRequestFlyingPrice += OnUIRequestFlyingPrice;
            _dispatcher.UIRequestFlyingText += OnUIRequestFlyingText;
            _dispatcher.UIRequestFlyingProduct += OnUIRequestFlyingProduct;
            _dispatcher.UIRequestFlyingExp += OnUIRequestFlyingExp;
            _dispatcher.CameraMoved += OnCameraMoved;
        }

        public void Unmediate()
        {
            _dispatcher.UIRequestFlyingPrice -= OnUIRequestFlyingPrice;
            _dispatcher.UIRequestFlyingText -= OnUIRequestFlyingText;
            _dispatcher.UIRequestFlyingProduct -= OnUIRequestFlyingProduct;
            _dispatcher.UIRequestFlyingExp -= OnUIRequestFlyingExp;
            _dispatcher.CameraMoved -= OnCameraMoved;
        }

        private void OnUIRequestFlyingExp(Vector2 screenPoint, int expAmount)
        {
            if (ScreenPointIsOnScreen(screenPoint) == true)
            {
                var icon = _spritesProvider.GetStarIcon(isBig: false);
                var view = CreateFlyingTextWithIcon(screenPoint, icon, expAmount);
                ShowAsFlyingText(view, screenPoint);
            }
        }

        private void OnUIRequestFlyingProduct(Vector2 screenPoint, string productKey, int amount)
        {
            if (ScreenPointIsOnScreen(screenPoint) == true)
            {
                var icon = _spritesProvider.GetProductIcon(productKey);
                var flyingPriceView = CreateFlyingTextWithIcon(screenPoint, icon, amount);
                ShowAsFlyingText(flyingPriceView, screenPoint, amount < 0);
            }
        }

        private void OnUIRequestFlyingPrice(Vector2 screenPoint, bool isGold, int amount)
        {
            if (ScreenPointIsOnScreen(screenPoint) == true)
            {
                var priceIcon = isGold ? _spritesProvider.GetGoldIcon() : _spritesProvider.GetCashIcon();
                var flyingPriceView = CreateFlyingTextWithIcon(screenPoint, priceIcon, amount);
                ShowAsFlyingText(flyingPriceView, screenPoint);
            }
        }

        private UIFlyingTextView CreateFlyingTextWithIcon(Vector2 screenPoint, Sprite image, int amount)
        {
            var flyingPriceGo = GameObject.Instantiate(_prefabsHolder.UIFlyingTextImagePrefab, _contentTransform);
            var flyingPriceView = flyingPriceGo.GetComponent<UIFlyingTextImageView>();
            if (_screenCalculator.ScreenPointToWorldPointInRectangle(_contentTransform, screenPoint, out var position))
            {
                flyingPriceGo.transform.position = position;
            }

            flyingPriceView.SetImageSprite(image);
            var amountStr = FormattingHelper.ToCommaSeparatedNumber(amount);
            flyingPriceView.SetText(amount >= 0 ? $"+{amountStr}" : amountStr);
            flyingPriceView.SetTextColor(amount >= 0 ? Color.green : Color.red);

            return flyingPriceView;
        }

        private void OnUIRequestFlyingText(Vector2 screenPoint, string text)
        {
            if (ScreenPointIsOnScreen(screenPoint) == true)
            {
                var flyingPriceView = CreateFlyingText(screenPoint, text);
                ShowAsFlyingText(flyingPriceView, screenPoint);
            }
        }

        private bool ScreenPointIsOnScreen(Vector2 screenPoint)
        {
            if (screenPoint.x > Screen.width || screenPoint.x < 0 || screenPoint.y > Screen.height || screenPoint.y < 0) return false;
            return true;
        }

        private UIFlyingTextView CreateFlyingText(Vector2 screenPoint, string text)
        {
            var flyingtextGo = GameObject.Instantiate(_prefabsHolder.UIFlyingTextPrefab, _contentTransform);
            var flyingTextView = flyingtextGo.GetComponent<UIFlyingTextView>();
            if (_screenCalculator.ScreenPointToWorldPointInRectangle(_contentTransform, screenPoint, out var position))
            {
                flyingtextGo.transform.position = position;
            }
            flyingTextView.SetText(text);
            return flyingTextView;
        }

        private void ShowAsFlyingText(UIFlyingTextView flyingTextView, Vector2 screenPoint, bool upDirection = true)
        {
            var flyingTextGo = flyingTextView.gameObject;

            var rectTransform = flyingTextGo.transform as RectTransform;
            var groundWorldPoint = _screenCalculator.ScreenPointToPlaneWorldPoint(screenPoint);
            _itemsWithOffsetsList.Add((rectTransform, groundWorldPoint));

            var duration = 2f;
            LeanTween.value(flyingTextGo, flyingTextView.SetAlpha, 1, 0, duration)
                .setEaseInQuad();
            LeanTween.value(flyingTextGo, flyingTextView.SetOffsetY, upDirection ? 0 : 150, upDirection ? 150 : 0, duration)
                .setEaseOutQuad()
                .setOnComplete(() => DestroyItem(flyingTextGo));
        }

        private void DestroyItem(GameObject itemGo)
        {
            var indexToRemove = -1;
            var transformToRemove = itemGo.transform;
            for (var i = 0; i < _itemsWithOffsetsList.Count; i++)
            {
                if (_itemsWithOffsetsList[i].transform == transformToRemove)
                {
                    indexToRemove = i;
                }
            }
            if (indexToRemove >= 0)
            {
                _itemsWithOffsetsList.RemoveAt(indexToRemove);
            }
            GameObject.Destroy(itemGo);
        }

        private void OnCameraMoved(Vector3 delta)
        {
            foreach (var (transform, worldPosition) in _itemsWithOffsetsList)
            {
                var screenPoint = _camera.WorldToScreenPoint(worldPosition);
                if (_screenCalculator.ScreenPointToWorldPointInRectangle(_contentTransform, screenPoint, out var position))
                {
                    transform.position = position;
                }
            }
        }
    }
}
