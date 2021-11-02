using Cysharp.Threading.Tasks;

public struct ActualizePlayerDataCommand : IAsyncGameLoadCommand
{
    public UniTask<bool> ExecuteAsync()
    {
        var mainConfig = GameConfigManager.Instance.MainConfig;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var billboardModel = playerModel.ShopModel.BillboardModel;
        var loc = LocalizationManager.Instance;

        if (billboardModel.IsAvailable == false)
        {
            if (playerModel.ProgressModel.Level >= mainConfig.BillboardUnlockLevel)
            {
                billboardModel.SetText(loc.GetLocalization(LocalizationKeys.BillboardDefaultText));
                billboardModel.SetAvailable(true);
            }
        }

        return UniTask.FromResult(true);
    }
}
