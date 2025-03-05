using Src.Common;
using Src.Managers;
using Src.Model;
using Src.Model.Configs;

namespace Src.Systems
{
    public class AdvertProcessCooldownSystem
    {
        private readonly GameStateModel _gameStateModel;
        private readonly UpdatesProvider _updatesProvider;
        private readonly AdvertViewStateModel _advertViewStateModel;
        //
        private IAdvertConfig _advertConfig;
        private int _advertWatchCooldownSeconds;

        public AdvertProcessCooldownSystem()
        {
            _gameStateModel = GameStateModel.Instance;
            _updatesProvider = UpdatesProvider.Instance;
            _advertViewStateModel = AdvertViewStateModel.Instance;
        }

        public async void Start()
        {
            await _gameStateModel.GameDataLoadedTask;

            _advertConfig = GameConfigManager.Instance.AdvertConfig;
            _advertWatchCooldownSeconds = _advertConfig.AdvertWatchCooldownMinutes * 60;
            Activate();
        }

        private void Activate()
        {
            _updatesProvider.RealtimeSecondUpdate += OnRealtimeSecondUpdate;
        }

        private void OnRealtimeSecondUpdate()
        {
            if (_advertViewStateModel.BankAdvertWatchesCount > 0
                && _gameStateModel.ServerTime > _advertViewStateModel.BankAdvertWatchTime + _advertWatchCooldownSeconds)
            {
                _advertViewStateModel.BankAdvertWatchesCount = 0;
            }
        }
    }
}
