using System;

public class UpdatesProvider
{
    private static Lazy<UpdatesProvider> _instance = new Lazy<UpdatesProvider>();
    public static UpdatesProvider Instance => _instance.Value;

    public event Action RealtimeUpdate = delegate { };
    public event Action GametimeUpdate = delegate { };

    public void CallRealtimeUpdate()
    {
        RealtimeUpdate();
    }

    public void CallGameplayUpdate()
    {
        GametimeUpdate();
    }
}