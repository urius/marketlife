public class UISaveIconMediator : IMediator
{
    private readonly UISaveIconView _view;
    private readonly Dispatcher _dispatcher;
    private readonly UpdatesProvider _updatesProvider;
    private readonly LocalizationManager _loc;

    private bool _isSaveInProgress;

    public UISaveIconMediator(UISaveIconView view)
    {
        _view = view;

        _dispatcher = Dispatcher.Instance;
        _updatesProvider = UpdatesProvider.Instance;
        _loc = LocalizationManager.Instance;
    }

    public void Mediate()
    {
        _view.SetText(_loc.GetLocalization(LocalizationKeys.CommonSaving));

        _view.gameObject.SetActive(false);
        _dispatcher.SaveStateChanged += OnSaveStateChanged;
    }

    public void Unmediate()
    {
        _dispatcher.SaveStateChanged -= OnSaveStateChanged;
        _updatesProvider.RealtimeUpdate -= OnRealtimeUpdate;
    }

    private void OnRealtimeUpdate()
    {
        if (_isSaveInProgress || true)
        {
            _view.ImageRectTransform.Rotate(0, 5, 2);
        }
    }

    private void OnSaveStateChanged(bool isSaving)
    {
        _isSaveInProgress = isSaving;
        _view.gameObject.SetActive(isSaving);

        _updatesProvider.RealtimeUpdate -= OnRealtimeUpdate;
        if (isSaving)
        {
            _updatesProvider.RealtimeUpdate += OnRealtimeUpdate;
        }
    }
}
