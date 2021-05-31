public abstract class PopupViewModelBase
{
    public abstract PopupType PopupType { get; }
}

public enum PopupType
{
    Unknown,
    ConfirmRemoveObject,
}
