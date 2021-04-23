using UnityEngine;

public abstract class HumanHeadViewBase : MonoBehaviour
{
    protected SpritesProvider SpritesProvider;

    virtual protected void Awake()
    {
        SpritesProvider = SpritesProvider.Instance;
    }

    public abstract void SetHair(int hairId);
}
