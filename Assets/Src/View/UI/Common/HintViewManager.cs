using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintViewManager
{
    private static Lazy<HintViewManager> _instance = new Lazy<HintViewManager>();
    public static HintViewManager Instance => _instance.Value;

    public UIHintView _hintView;

    public void Show(Transform parentTransform, HintPositionType positionType, string localizationKey, float maxHintBGWidth, Vector2 positionOffset)
    {
        if (_hintView == null)
        {
            var hintGo = GameObject.Instantiate(PrefabsHolder.Instance.UIHintPrefab);
            _hintView = hintGo.GetComponent<UIHintView>();
        }

        _hintView.gameObject.SetActive(true);

        _hintView.transform.SetParent(parentTransform, false);
        var text = LocalizationManager.Instance.GetLocalization(localizationKey);
        _hintView.SetParams(text, positionType, maxHintBGWidth);
        (_hintView.transform as RectTransform).anchoredPosition = positionOffset;
    }

    public void Hide()
    {
        if (_hintView != null)
        {
            _hintView.transform.SetParent(PoolCanvasProvider.Instance.PoolCanvasTransform);
            _hintView.gameObject.SetActive(false);
        }
    }
}
