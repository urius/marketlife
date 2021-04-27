using System.Threading;
using UnityEngine;

public abstract class UINotMonoMediatorBase<TView>
    where TView : MonoBehaviour
{
    protected RectTransform ContentTransform;
    protected TView View;    

    private CancellationTokenSource _stopMediationCts;
    protected CancellationToken StopMediationToken => _stopMediationCts.Token;

    public void Mediate(TView view)
    {
        View = view;

        Mediate(view.transform as RectTransform);
    }

    public void Mediate(RectTransform contentTransform)
    {
        _stopMediationCts = new CancellationTokenSource();

        ContentTransform = contentTransform;

        OnStart();
    }

    public void Unmediate()
    {
        _stopMediationCts.Cancel();
        OnStop();
        _stopMediationCts.Dispose();
    }

    protected abstract void OnStart();
    protected abstract void OnStop();
}
