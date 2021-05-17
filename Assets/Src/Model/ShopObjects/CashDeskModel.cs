using UnityEngine;

public class CashDeskModel : ShopObjectBase
{
    public CashDeskModel(int numericId, ShopObjectConfigDto cashDeskConfigDto, Vector2Int coords, int side = 3, string paramsShort = null)
        : base(numericId, cashDeskConfigDto, coords, side)
    {
    }

    public override ShopObjectType Type => ShopObjectType.CashDesk;

    public override ShopObjectBase Clone()
    {
        return new ShopObjectModelFactory().CreateCashDesk(NumericId, Coords, Side);
    }
}
