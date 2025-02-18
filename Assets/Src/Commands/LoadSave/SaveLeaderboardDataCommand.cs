using Newtonsoft.Json;
using Src.Common;
using UnityEngine;

public struct SaveLeaderboardDataCommand
{
    public async void Execute()
    {
#if UNITY_EDITOR
        if (DebugDataHolder.Instance.IsSaveDisabled == true) return;
#endif
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var friendsDataHolder = FriendsDataHolder.Instance;
        var dataExporter = DataExporter.Instance;
        var urlsHolder = Urls.Instance;
        var url = string.Format(Urls.SaveLeaderboardDataURL, playerModel.Uid);

        var dataToSave = dataExporter.ExportLeaderboardPlayerData(playerModel.ProgressModel, friendsDataHolder.InGameFriendsCount);
        var dataToSaveStr = JsonConvert.SerializeObject(dataToSave);

        var resultOperation = await new WebRequestsSender().PostAsync<CommonResponseDto>(url, dataToSaveStr);

        if (resultOperation.IsSuccess == false)
        {
            Debug.Log($"{nameof(SaveLeaderboardDataCommand)} Save leaderboard data failed");
        }
    }
}
