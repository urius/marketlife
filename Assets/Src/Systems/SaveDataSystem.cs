using System;

public class SaveDataSystem
{
    private readonly Dispatcher _dispatcher;
    private readonly GameStateModel _gameStateModel;

    private SaveField _saveFieldsData = SaveField.None;

    public SaveDataSystem()
    {
        _dispatcher = Dispatcher.Instance;
        _gameStateModel = GameStateModel.Instance;
    }

    public async void Start()
    {
        await _gameStateModel.GameDataLoadedTask;

        Activate();
    }

    public void MarkToSaveField(SaveField field)
    {
        _saveFieldsData |= field;
    }

    private void Activate()
    {
    }
}

[Flags]
public enum SaveField
{
    None = 0,
    Progress = 1 << 0,
    Personal = 1 << 1,
    Warehouse = 1 << 2,
    Design = 1 << 3,
    ShopObjects = 1 << 4,
    All = Progress | Personal | Warehouse | Design | ShopObjects,
}
