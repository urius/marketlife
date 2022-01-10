using System;

public class DailyMissionChangeCashmanProcessor : DailyMissionProcessorBase
{
    private readonly GameStateModel _gameStateModel;

    private CashmanDressData _cashmanDressData;

    public DailyMissionChangeCashmanProcessor()
    {
        _gameStateModel = GameStateModel.Instance;
    }

    public override void Start()
    {
        _gameStateModel.PopupShown += OnPopupShown;
        _gameStateModel.PopupRemoved += OnPopupRemoved;
    }

    public override void Stop()
    {
        _gameStateModel.PopupShown -= OnPopupShown;
        _gameStateModel.PopupRemoved -= OnPopupRemoved;
    }

    private void OnPopupShown()
    {
        if (_gameStateModel.ShowingPopupModel.PopupType == PopupType.CashDesk)
        {
            var cashDeskModel = (_gameStateModel.ShowingPopupModel as CashDeskPopupViewModel).CashDeskModel;
            _cashmanDressData = GetCashmanDressData(cashDeskModel);
        }
    }

    private void OnPopupRemoved(PopupViewModelBase popupViewModel)
    {
        if (popupViewModel.PopupType == PopupType.CashDesk)
        {
            var cashDeskModel = (popupViewModel as CashDeskPopupViewModel).CashDeskModel;
            var cashmanDressDataNew = GetCashmanDressData(cashDeskModel);

            if (_cashmanDressData.Equals(cashmanDressDataNew) == false)
            {
                MissionModel.AddValue(1);
            }
        }
    }

    private CashmanDressData GetCashmanDressData(CashDeskModel cashDeskModel)
    {
        return new CashmanDressData
        {
            HairId = cashDeskModel.HairId,
            GlassesId = cashDeskModel.GlassesId,
            DressId = cashDeskModel.DressId,
        };
    }
}

public struct CashmanDressData
{
    public int HairId;
    public int GlassesId;
    public int DressId;
}
