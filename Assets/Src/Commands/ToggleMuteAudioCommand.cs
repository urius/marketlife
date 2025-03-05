using Src.Model;

namespace Src.Commands
{
    public struct ToggleMuteAudioCommand
    {
        public void Execute()
        {
            var playerModel = PlayerModelHolder.Instance.UserModel;
            var settingsModel = playerModel.UserSettingsModel;
            settingsModel.SetAudioMutedState(!settingsModel.IsAudioMuted);
        }
    }
}
