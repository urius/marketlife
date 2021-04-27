using UnityEngine;

public class CashDeskModel : ShopObjectBase
{
    public CashDeskModel(int level, ShopObjectConfigDto cashDeskConfigDto, Vector2Int coords, int side = 3, string paramsShort = null)
        : base(level, cashDeskConfigDto, coords, side)
    {
    }

    public override ShopObjectType Type => ShopObjectType.CashDesk;
}
