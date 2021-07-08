using System;
using UnityEngine;
using UnityEngine.UI;

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
}
