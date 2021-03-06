using UnityEngine;
using UnityEngine.UI;

public class UIFlyingTextImageView : UIFlyingTextView
{
    [SerializeField] private Image _image;

    public void SetImageSprite(Sprite sprite)
    {
        _image.sprite = sprite;
    }

    public override void SetAlpha(float alpha)
    {
        base.SetAlpha(alpha);

        _image.color = _image.color.SetAlpha(alpha);
    }
}
