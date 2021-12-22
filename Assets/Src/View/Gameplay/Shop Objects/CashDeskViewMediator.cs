using UnityEngine;

public class CashDeskViewMediator : ShopObjectMediatorBase
{
    private readonly SpritesProvider _spritesProvider;
    private readonly CashDeskModel _cashDeskModel;

    public CashDeskViewMediator(Transform parentTransform, CashDeskModel cashDeskModel)
        : base(parentTransform, cashDeskModel)
    {
        _cashDeskModel = cashDeskModel;

        _spritesProvider = SpritesProvider.Instance;
    }

    public override void Mediate()
    {
        base.Mediate();

        Activate();
    }

    public override void Unmediate()
    {
        Deactivate();

        base.Unmediate();
    }

    protected override void UpdateView()
    {
        base.UpdateView();

        if (DateTimeHelper.IsNewYearsEve())
        {
            (CurrentView as CashDeskView).SetHatSprite(_spritesProvider.GetSantaHatSprite());
        }
        else
        {
            (CurrentView as CashDeskView).SetHatSprite(null);
        }

        UpdateCashManView();
    }

    private void Activate()
    {
        _cashDeskModel.DisplayItemChanged += OnItemChanged;
    }

    private void Deactivate()
    {
        _cashDeskModel.DisplayItemChanged -= OnItemChanged;
    }

    private void OnItemChanged()
    {
        UpdateCashManView();
    }

    private void UpdateCashManView()
    {
        var view = CurrentView as CashDeskView;
        view.HeadView.SetHair(_cashDeskModel.HairId);
        view.HeadView.SetGlasses(_cashDeskModel.GlassesId);
        view.SetTorsoSprite(_spritesProvider.GetTopDressSprite(_cashDeskModel.DressId));
        view.SetHandsSprite(_spritesProvider.GetHandDressSprite(_cashDeskModel.DressId));
    }
}
