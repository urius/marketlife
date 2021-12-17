using UnityEngine;

public class CashDeskViewMediator : ShopObjectMediatorBase
{
    private readonly SpritesProvider _spritesProvider;

    public CashDeskViewMediator(Transform parentTransform, CashDeskModel cashDeskModel)
        : base(parentTransform, cashDeskModel)
    {
        _spritesProvider = SpritesProvider.Instance;
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
    }
}
