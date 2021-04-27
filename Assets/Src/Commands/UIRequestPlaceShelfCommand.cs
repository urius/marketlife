public struct UIRequestPlaceShelfCommand
{
    public void Execute(int shelfId)
    {
        var shelfConfig = GameConfigManager.Instance.MainConfig.GetShelfConfigById(shelfId);
        var gameStateModel = GameStateModel.Instance;

        if (shelfConfig.price != null)// TODO check price properly
        {
            //gameStateModel.SetPlacingObject(PlacingObjectType.Shelf, shelfId);
        }
    }
}
