using System;

public abstract class PopupViewModelBase : IDisposable
{
    public abstract PopupType PopupType { get; }

    public virtual void Dispose()
    {
    }
}

public enum PopupType
{
    Unknown,
    Confirm,
    OrderProduct,
    ShelfContent,
    WarehouseForShelf,
    Warehouse,
    Upgrades,
    OfflineReport,
    LevelUp,
    Bank,
    OldGameCompensation,
    Error,
    DailyBonus,
    Billboard,
    CashDesk,
}
