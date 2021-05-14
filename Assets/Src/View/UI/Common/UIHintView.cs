using TMPro;
using UnityEngine;

public class UIHintView : MonoBehaviour
{
    [SerializeField] private HintPositionType _positionType;
    public HintPositionType PositionType => _positionType;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private RectTransform _bgRect;
    [SerializeField] private RectTransform _arrowRect;

    private const int DefaultBodyOffset = 20;

    public void Start()
    {
        RecalculateBounds();
        SetPositionType(_positionType);
    }

    public void SetParams(string text, HintPositionType positionType)
    {
        _text.text = text;

        RecalculateBounds();
        SetPositionType(positionType);
    }

    public void SetText(string text)
    {
        _text.text = text;
        RecalculateBounds();
    }

    public void SetPositionType(HintPositionType positionType)
    {
        _positionType = positionType;
        switch (positionType)
        {
            case HintPositionType.Up:
                _arrowRect.localEulerAngles = new Vector3(0, 0, 180);
                _bgRect.pivot = new Vector2(0.5f, 0);
                _bgRect.anchoredPosition = new Vector2(0, DefaultBodyOffset);
                break;
            case HintPositionType.Down:
                _arrowRect.localEulerAngles = new Vector3(0, 0, 0);
                _bgRect.pivot = new Vector2(0.5f, 1);
                _bgRect.anchoredPosition = new Vector2(0, -DefaultBodyOffset);
                break;
            case HintPositionType.Right:
                _arrowRect.localEulerAngles = new Vector3(0, 0, 90);
                _bgRect.pivot = new Vector2(0, 0.5f);
                _bgRect.anchoredPosition = new Vector2(DefaultBodyOffset, 0);
                break;
            case HintPositionType.Left:
                _arrowRect.localEulerAngles = new Vector3(0, 0, -90);
                _bgRect.pivot = new Vector2(1, 0.5f);
                _bgRect.anchoredPosition = new Vector2(-DefaultBodyOffset, 0);
                break;
        }
    }

    private void RecalculateBounds()
    {
        _text.enableWordWrapping = false;

        var size = _bgRect.sizeDelta;
        _bgRect.sizeDelta = size;

        _text.ForceMeshUpdate();

        size = _bgRect.sizeDelta;
        var textRectTransform = _text.rectTransform;
        size.x = _text.textBounds.size.x + textRectTransform.offsetMin.x - textRectTransform.offsetMax.x + 1;
        size.y = _text.textBounds.size.y + textRectTransform.offsetMin.y - textRectTransform.offsetMax.y;
        _bgRect.sizeDelta = size;
    }
}

public enum HintPositionType
{
    Up,
    Down,
    Left,
    Right,
}
