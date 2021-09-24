using System;

public class UISaveIconMediator : IMediator
{
    private readonly UISaveIconView _view;
    private readonly Dispatcher _dispatcher;
    private readonly UpdatesProvider _updatesProvider;
    private readonly LocalizationManager _loc;
    private readonly TutorialUIElementsProvider _tutorialUIElementsProvider;
    private bool _isSaveInProgress;
    private bool _forceShowSaveIcon;

    public UISaveIconMediator(UISaveIconView view)
    {
        _view = view;

        _dispatcher = Dispatcher.Instance;
        _updatesProvider = UpdatesProvider.Instance;
        _loc = LocalizationManager.Instance;
        _tutorialUIElementsProvider = TutorialUIElementsProvider.Instance;
    }

    public void Mediate()
    {
        _tutorialUIElementsProvider.SetElement(TutorialUIElement.TopSaveIcon, _view.transform);

        _view.SetText(_loc.GetLocalization(LocalizationKeys.CommonSaving));

        _view.gameObject.SetActive(false);
        _dispatcher.SaveStateChanged += OnSaveStateChanged;
        _dispatcher.TutorialSaveStateChanged += OnTutorialSaveStateChanged;
    }

    public void Unmediate()
    {
        _tutorialUIElementsProvider.ClearElement(TutorialUIElement.TopSaveIcon);

        _dispatcher.SaveStateChanged -= OnSaveStateChanged;
        _dispatcher.TutorialSaveStateChanged -= OnTutorialSaveStateChanged;
        _updatesProvider.RealtimeUpdate -= OnRealtimeUpdate;
    }

    private void OnRealtimeUpdate()
    {
        if (_isSaveInProgress || true)
        {
            _view.ImageRectTransform.Rotate(0, 5, 2);
        }
    }

    private void OnTutorialSaveStateChanged(bool isSaving)
    {
        _forceShowSaveIcon = isSaving;
        OnSaveStateChanged(isSaving);
    }

    private void OnSaveStateChanged(bool isSaving)
    {
        _isSaveInProgress = isSaving || _forceShowSaveIcon;
        _view.gameObject.SetActive(_isSaveInProgress);

        _updatesProvider.RealtimeUpdate -= OnRealtimeUpdate;
        if (_isSaveInProgress)
        {
            _updatesProvider.RealtimeUpdate += OnRealtimeUpdate;
        }
    }
}
