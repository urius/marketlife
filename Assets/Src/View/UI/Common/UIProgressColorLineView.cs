using System;
using UnityEngine;
using UnityEngine.UI;

namespace Src.View.UI.Common
{
    public class UIProgressColorLineView : MonoBehaviour
    {
        private static readonly Color[] TimeProgressColors = { Color.red, Color.red, Color.yellow, Color.yellow, Color.yellow, Color.green, Color.green  };
        
        [SerializeField] private RectTransform _progressRectTransform;
        [SerializeField] private Image _progressRectTransformImage;

        public void SetProgress(float progress)
        {
            _progressRectTransform.anchorMax = new Vector2(progress, _progressRectTransform.anchorMax.y);
            _progressRectTransform.anchorMin = new Vector2(0, _progressRectTransform.anchorMin.y);
            _progressRectTransform.offsetMax = Vector2.zero;
            _progressRectTransform.offsetMin = Vector2.zero;

            _progressRectTransformImage.color = GetColorByProgress(progress);
        }

        private static Color GetColorByProgress(float progress)
        {
            if (progress >= 1)
            {
                return Color.green;
            }
            
            var colorIndex = progress * (TimeProgressColors.Length - 1);
            var index = (int)colorIndex;
        
            index = Math.Clamp(index, 0, TimeProgressColors.Length - 1);
            
            return TimeProgressColors[index];
        }
    }
}