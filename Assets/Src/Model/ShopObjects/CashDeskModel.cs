public class CashDeskModel : ShopObjectBase
{
    public CashDeskModel(int level, int angle, string paramsShort) : base(level, angle)
    {
    }

    public override ShopObjectType Type => ShopObjectType.CashDesk;
}
