using System;
using UnityEngine;

public class CashDeskModel : ShopObjectModelBase
{
    public event Action DisplayItemChanged = delegate { };

    public CashDeskModel(
        int numericId,
        ShopObjectConfigDto cashDeskConfigDto,
        Vector2Int coords,
        int side,
        int hairId,
        int glassesId,
        int dressId)
        : base(numericId, cashDeskConfigDto, coords, side)
    {
        HairId = hairId;
        GlassesId = glassesId;
        DressId = dressId;
    }

    public int HairId { get; private set; }
    public int GlassesId { get; private set; }
    public int DressId { get; private set; }
    public override ShopObjectType Type => ShopObjectType.CashDesk;

    public void SetHairId(int id)
    {
        HairId = id;
        DisplayItemChanged();
    }

    public void SetGlassesId(int id)
    {
        GlassesId = id;
        DisplayItemChanged();
    }

    public void SetDressId(int id)
    {
        DressId = id;
        DisplayItemChanged();
    }

    public override ShopObjectModelBase Clone()
    {
        return new ShopObjectModelFactory().CreateCashDesk(NumericId, Coords, Side);
    }
}
