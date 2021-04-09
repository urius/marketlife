public abstract class ShopObjectBase
{
    public readonly int Level;

    public ShopObjectBase(int level, int angle)
    {
        Level = level;

        Angle = angle;
    }

    public int Angle { get; private set; }

    public abstract ShopObjectType Type { get; }
}

public enum ShopObjectType
{
    Undefined,
    CashDesk,
    Shelf,
}
