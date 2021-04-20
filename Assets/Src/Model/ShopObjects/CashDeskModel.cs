using UnityEngine;

public class CashDeskModel : ShopObjectBase
{
    public CashDeskModel(ShopObjectConfigDto cashDeskConfigDto, Vector2Int coords, int level, int angle, string paramsShort)
        : base(coords, level, angle, cashDeskConfigDto)
    {
    }

    public override ShopObjectType Type => ShopObjectType.CashDesk;
}
