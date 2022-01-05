public class UIHandleCashDeskPopupChangeItemClickCommand
{
    public void Execute(CashDeskCashManItemType itemType, int itemId)
    {
        var gameStateModel = GameStateModel.Instance;
        var spritesProvider = SpritesProvider.Instance;

        var currentCashDeskModel = (gameStateModel.ShowingPopupModel as CashDeskPopupViewModel).CashDeskModel;
        switch (itemType)
        {
            case CashDeskCashManItemType.Hair:
                itemId = ProcessBounds(itemId, spritesProvider.GetMaxHairId());
                currentCashDeskModel.SetHairId(itemId);
                break;
            case CashDeskCashManItemType.Glasses:
                itemId = ProcessBounds(itemId, spritesProvider.GetMaxGlassesId(), minId: 0);
                currentCashDeskModel.SetGlassesId(itemId);
                break;
            case CashDeskCashManItemType.Dress:
                itemId = ProcessBounds(itemId, spritesProvider.GetMaxBottomDressId());
                currentCashDeskModel.SetDressId(itemId);
                break;
        }
    }

    private int ProcessBounds(int itemId, int maxId, int minId = 1)
    {
        var result = itemId;
        if (itemId > maxId)
        {
            result = minId;
        }
        else if (itemId < minId)
        {
            result = maxId;
        }

        return result;
    }
}

public enum CashDeskCashManItemType
{
    None,
    Hair,
    Glasses,
    Dress,
}
