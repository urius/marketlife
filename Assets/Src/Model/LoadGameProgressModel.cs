using System;

public class LoadGameProgressModel
{
    public static readonly LoadGameProgressModel Instance = new LoadGameProgressModel();

    public event Action<float> ProgressChanged = delegate { };
    public event Action<LoadGamePhase> PhaseChanged = delegate { };
    public event Action ErrorHappened = delegate { };

    private int _partsNumTotal = 1;
    private int _partsNumLoaded = 0;
    private float _currentPartLoadProgress = 0;

    public float LoadProgress => (_partsNumLoaded + _currentPartLoadProgress) / _partsNumTotal;
    public LoadGamePhase PhaseName { get; private set; }
    public bool IsError { get; private set; } = false;

    public void SetupPartsCount(int partsCount)
    {
        _partsNumTotal = partsCount;
    }

    public void SetCurrentPhaseName(LoadGamePhase name)
    {
        PhaseName = name;
        PhaseChanged(PhaseName);
    }

    public void SetCurrentPartLoadProgress(float progress)
    {
        _currentPartLoadProgress = progress;
        ProgressChanged(LoadProgress);
    }

    public void SetCurrentPartLoaded()
    {
        _partsNumLoaded++;
        _currentPartLoadProgress = 0;
        if (_partsNumLoaded > _partsNumTotal)
        {
            _partsNumTotal = _partsNumLoaded;
        }
        ProgressChanged(LoadProgress);
    }

    public void SetErrorState()
    {
        IsError = true;
        ErrorHappened();
    }
}

public enum LoadGamePhase
{
    Undefined,
    LoadTime,
    LoadLocalization,
    LoadConfigs,
    LoadAssets,
    LoadShopData,
    LoadCompensationData,
}
