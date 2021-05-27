using System.Collections.Generic;
using UnityEngine;

public class UIFlyingTextsMediator : IMediator
{
    private readonly RectTransform _contentTransform;
    private readonly Dispatcher _dispatcher;
    private readonly ScreenCalculator _screenCalculator;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly SpritesProvider _spritesProvider;
    private readonly UpdatesProvider _updatesProvider;
    private readonly GridCalculator _gridCalculator;
    private readonly List<(RectTransform transform, Vector3 worldPosition)> _itemsWithOffsetsList = new List<(RectTransform, Vector3)>(5);

    private Camera _camera;

    public UIFlyingTextsMediator(RectTransform contentTransform)
    {
        _contentTransform = contentTransform;

        _dispatcher = Dispatcher.Instance;
        _screenCalculator = ScreenCalculator.Instance;
        _prefabsHolder = PrefabsHolder.Instance;
        _spritesProvider = SpritesProvider.Instance;
        _updatesProvider = UpdatesProvider.Instance;
        _gridCalculator = GridCalculator.Instance;
    }

    public void Mediate()
    {
        _camera = Camera.main;

        _dispatcher.UIRequestFlyingPrice += OnUIRequestFlyingPrice;
        _dispatcher.UIRequestFlyingText += OnUIRequestFlyingText;
        _dispatcher.CameraMoved += OnCameraMoved;
    }

    public void Unmediate()
    {
        _dispatcher.UIRequestFlyingPrice -= OnUIRequestFlyingPrice;
        _dispatcher.UIRequestFlyingText -= OnUIRequestFlyingText;
        _dispatcher.CameraMoved -= OnCameraMoved;
    }

    private void OnUIRequestFlyingPrice(Vector2 screenPoint, bool isGold, int amount)
    {
        var flyingPriceView = CreateFlyingPrice(screenPoint, isGold, amount);
        ShowAsFlyingText(flyingPriceView, screenPoint);
    }

    private UIFlyingTextView CreateFlyingPrice(Vector2 screenPoint, bool isGold, int amount)
    {
        var flyingPriceGo = GameObject.Instantiate(_prefabsHolder.UIFlyingPricePrefab, _contentTransform);
        var flyingPriceView = flyingPriceGo.GetComponent<UIFlyingPriceView>();
        if (_screenCalculator.ScreenPointToWorldPointInRectangle(_contentTransform, screenPoint, out var position))
        {
            flyingPriceGo.transform.position = position;
        }

        flyingPriceView.SetImageSprite(isGold ? _spritesProvider.GetGoldIcon() : _spritesProvider.GetCashIcon());
        var amountStr = string.Format("{0:n0}", amount);
        flyingPriceView.SetText(amount >= 0 ? $"+{amountStr}" : amountStr);
        flyingPriceView.SetTextColor(amount >= 0 ? Color.green : Color.red);

        return flyingPriceView;
    }

    private void OnUIRequestFlyingText(Vector2 screenPoint, string text)
    {
        var flyingPriceView = CreateFlyingText(screenPoint, text);
        ShowAsFlyingText(flyingPriceView, screenPoint);
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

    private void ShowAsFlyingText(UIFlyingTextView flyingTextView, Vector2 screenPoint)
    {
        var flyingTextGo = flyingTextView.gameObject;

        var rectTransform = flyingTextGo.transform as RectTransform;
        var groundWorldPoint = _gridCalculator.ScreenPointToPlaneWorldPoint(_camera, screenPoint);
        _itemsWithOffsetsList.Add((rectTransform, groundWorldPoint));

        var duration = 2f;
        LeanTween.value(flyingTextGo, flyingTextView.SetAlpha, 1, 0, duration)
            .setEaseInQuad();
        LeanTween.value(flyingTextGo, flyingTextView.SetOffsetY, 0, 150, duration)
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
