using UnityEngine;

public abstract class UINotMonoMediatorBase : IMediator
{
    protected readonly RectTransform ContentTransform;

    public UINotMonoMediatorBase(RectTransform contentTransform)
    {
        ContentTransform = contentTransform;
    }

    public virtual void Mediate()
    {
    }

    public virtual void Unmediate()
    {
    }
}
