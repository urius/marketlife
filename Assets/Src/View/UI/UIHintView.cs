using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIHintView : MonoBehaviour
{
    [SerializeField] private HintPositionType _positionType;
    [SerializeField] private float _maxWidth;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private RectTransform _bgRect;
    [SerializeField] private RectTransform _arrowRect;

    private const int DefaultBodyOffset = 20;

    void Start()
    {
        _text.ForceMeshUpdate();
        var size = _bgRect.sizeDelta;
        size.y = _text.textBounds.size.y;
        _bgRect.sizeDelta = size;
        //Debug.Log(_bgRect.sizeDelta);

        SetPositionType(HintPositionType.Left);
    }

    public void SetPositionType(HintPositionType positionType)
    {
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
}

public enum HintPositionType
{
    Up,
    Down,
    Left,
    Right,
}
