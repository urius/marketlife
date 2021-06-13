using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIOrderProductFromPopupAnimator
{
    private const float AnimationDurationSeconds = 0.8f;

    private readonly RectTransform _parentTransform;
    private readonly Vector2 _startAnimationScreenPosition;
    private readonly UIBottomPanelScrollItemView _slotView;
    private readonly ProductModel _productModel;
    private readonly SpritesProvider _spritesProvider;
    private readonly ScreenCalculator _screenCalculator;

    private Image _flyingProductImage;
    private TaskCompletionSource<bool> _animationTcs;
    private LTDescr _tweenDescriptionX;
    private LTDescr _tweenDescriptionY;

    public UIOrderProductFromPopupAnimator(
        RectTransform parentTransform,
        Vector2 startAnimationScreenPosition,
        UIBottomPanelScrollItemView slotView,
        ProductModel productModel)
    {
        _parentTransform = parentTransform;
        _startAnimationScreenPosition = startAnimationScreenPosition;
        _slotView = slotView;
        _productModel = productModel;

        _spritesProvider = SpritesProvider.Instance;
        _screenCalculator = ScreenCalculator.Instance;
    }

    public Task AnimationTask => _animationTcs?.Task ?? Task.CompletedTask;

    public Task AnimateAsync()
    {
        var go = new GameObject("UIFlyingProduct", typeof(Image));
        go.transform.SetParent(_parentTransform, false);
        _flyingProductImage = go.GetComponent<Image>();
        _flyingProductImage.preserveAspect = true;

        _flyingProductImage.sprite = _spritesProvider.GetProductIcon(_productModel.Config.Key);
        if (_screenCalculator.ScreenPointToWorldPointInRectangle(_parentTransform, _startAnimationScreenPosition, out var position))
        {
            _animationTcs = new TaskCompletionSource<bool>();
            _flyingProductImage.transform.position = position;
            var finishLocalPosition = GetFinishLocalPosition();

            _tweenDescriptionX = LeanTween.moveLocalX(go, finishLocalPosition.x, AnimationDurationSeconds).setEaseInOutQuad();
            _tweenDescriptionY = LeanTween.moveLocalY(go, finishLocalPosition.y, AnimationDurationSeconds).setEaseInBack()
                .setOnComplete(OnAnimationComplete);
            return _animationTcs.Task;
        }
        else
        {
            GameObject.Destroy(go);
            return Task.CompletedTask;
        }
    }

    public void CancelAnimation()
    {
        LeanTween.cancel(_flyingProductImage.gameObject, true);
    }

    //not work properly
    public void UpdateFinishPosition()
    {
        var newFinishosition = GetFinishLocalPosition();
        _tweenDescriptionX.setTo(new Vector3(newFinishosition.x, 0, 0));
        _tweenDescriptionY.setTo(new Vector3(0, newFinishosition.y, 0));
    }

    private void OnAnimationComplete()
    {
        _animationTcs.TrySetResult(true);
        GameObject.Destroy(_flyingProductImage.gameObject);
    }

    private Vector3 GetFinishLocalPosition()
    {
        var finishScreenPosition = _screenCalculator.WorldToScreenPoint(_slotView.transform.position);
        if (_screenCalculator.ScreenPointToWorldPointInRectangle(_parentTransform, finishScreenPosition, out var position))
        {
            return _parentTransform.InverseTransformPoint(position);
        }

        return Vector3.zero;
    }
}
