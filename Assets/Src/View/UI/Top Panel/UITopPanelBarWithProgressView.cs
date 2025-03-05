using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Src.View.UI.Top_Panel
{
    public class UITopPanelBarWithProgressView : UITopPanelBarView
    {
        [SerializeField] private Image _progressImage;

        public void SetProgressBarImageSprite(Sprite sprite)
        {
            _progressImage.sprite = sprite;
        }

        public void SetProgress(float progress)
        {
            _progressImage.transform.localScale = new Vector3(Math.Max(0, progress), 1, 1);
        }

        public void SetProgressAnimated(float progress)
        {
            LeanTween.cancel(_progressImage.gameObject);
            _progressImage.transform.LeanScaleX(Math.Max(0, progress), ChangeAmountDuration);
        }

        public UniTask SetProgressAnimatedAsync(float progress, float duration = -1)
        {
            if (duration < 0)
            {
                duration = ChangeAmountDuration;
            }
            var result = new UniTaskCompletionSource();
            LeanTween.cancel(_progressImage.gameObject);
            _progressImage.transform.LeanScaleX(Math.Max(0, progress), duration);
            LeanTween.delayedCall(duration, () => result.TrySetResult());
            return result.Task;
        }
    }
}
