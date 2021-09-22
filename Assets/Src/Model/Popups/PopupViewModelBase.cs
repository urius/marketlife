public abstract class PopupViewModelBase
{
    public abstract PopupType PopupType { get; }
}

public enum PopupType
{
    Unknown,
    Confirm,
    OrderProduct,
    ShelfContent,
    Warehouse,
    Upgrades,
    OfflineReport,
    LevelUp,
    Bank,
    OldGameCompensation,
    Error,
    DailyBonus,
}
