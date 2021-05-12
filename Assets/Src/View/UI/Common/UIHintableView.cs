using UnityEngine;
using UnityEngine.EventSystems;

public class UIHintableView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private HintPositionType _hintPositionType;
    [SerializeField] private string _localizationKey;
    [SerializeField] private RectTransform _hintContainer;
    [SerializeField] private Vector2 _positionOffset;
    [SerializeField] private float _maxBGWidth = 220;

    public void OnPointerEnter(PointerEventData eventData)
    {
        HintViewManager.Instance.Show(transform, _hintPositionType, _localizationKey, _maxBGWidth, _positionOffset);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HintViewManager.Instance.Hide();
    }
}
