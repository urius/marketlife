using UnityEngine;

public class CashDeskModel : ShopObjectBase
{
    public CashDeskModel(Vector2Int coords, int level, int angle, string paramsShort)
        : base(coords, level, angle)
    {
    }

    public override ShopObjectType Type => ShopObjectType.CashDesk;
}
