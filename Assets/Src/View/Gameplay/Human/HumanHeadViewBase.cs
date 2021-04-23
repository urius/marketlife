using UnityEngine;

public abstract class HumanHeadViewBase : MonoBehaviour
{
    protected GraphicsManager GraphicsManager;

    virtual protected void Awake()
    {
        GraphicsManager = GraphicsManager.Instance;
    }

    public abstract void SetHair(int hairId);
}
