using UnityEngine;

public abstract class HumanHeadViewBase : MonoBehaviour
{
    protected SpritesProvider SpritesProvider => SpritesProvider.Instance;

    public abstract void SetHair(int hairId);
    public abstract void SetHatSprite(Sprite hatSprite);
}
