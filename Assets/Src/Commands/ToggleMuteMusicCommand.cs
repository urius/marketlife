public struct ToggleMuteMusicCommand
{
    public void Execute()
    {
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var settingsModel = playerModel.UserSettingsModel;
        settingsModel.SetMusicMutedState(!settingsModel.IsMusicMuted);
    }
}
