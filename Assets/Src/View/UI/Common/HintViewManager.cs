using System;
using Src.Common;
using Src.Managers;
using UnityEngine;

namespace Src.View.UI.Common
{
    public class HintViewManager
    {
        private static Lazy<HintViewManager> _instance = new Lazy<HintViewManager>();
        public static HintViewManager Instance => _instance.Value;

        public UIHintView _hintView;

        public void ShowLocalizable(Transform parentTransform, HintPositionType positionType, string localizationKey, Vector2 positionOffset)
        {
            var text = LocalizationManager.Instance.GetLocalization(localizationKey);
            ShowText(parentTransform, positionType, text, positionOffset);
        }

        public void ShowText(Transform parentTransform, HintPositionType positionType, string text, Vector2 positionOffset)
        {
            if (_hintView == null)
            {
                var hintGo = GameObject.Instantiate(PrefabsHolder.Instance.UIHintPrefab);
                _hintView = hintGo.GetComponent<UIHintView>();
            }

            _hintView.gameObject.SetActive(true);

            _hintView.transform.SetParent(parentTransform, false);
            _hintView.SetParams(text, positionType);
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
}
