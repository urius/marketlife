using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LeanTweenHelper
{
    public static void BounceX(RectTransform rectTransform, float deltaX, float duration1 = 0.3f, float duration2 = 0.6f)
    {
        var startPos = rectTransform.anchoredPosition.x;
        LeanTween.moveX(rectTransform, rectTransform.anchoredPosition.x + deltaX, duration1)
            .setEaseOutQuad()
            .setOnComplete(() => rectTransform.LeanMoveX(startPos, duration2).setEaseOutBounce());
    }

    public static UniTask BounceXAsync(RectTransform rectTransform, float deltaX, float duration1 = 0.3f, float duration2 = 0.6f)
    {
        var tcs = new UniTaskCompletionSource();

        var startPos = rectTransform.anchoredPosition.x;
        LeanTween.moveX(rectTransform, rectTransform.anchoredPosition.x + deltaX, duration1)
            .setEaseOutQuad()
            .setOnComplete(() => rectTransform.LeanMoveX(startPos, duration2).setEaseOutBounce().setOnComplete(() => tcs.TrySetResult()));
            
        return tcs.Task;
    }

    public static (UniTask task, LTDescr tweenDescription) MoveAsync(RectTransform rectTransform, Vector3 to, float duration)
    {
        var tcs = new UniTaskCompletionSource();
        var tweenDescription = LeanTween.move(rectTransform, to, duration)
            .setEaseOutBounce()
            .setOnComplete(() => tcs.TrySetResult());

        return (tcs.Task, tweenDescription);
    }
}
