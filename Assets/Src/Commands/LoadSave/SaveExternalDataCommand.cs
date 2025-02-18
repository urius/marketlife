using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Src.Common;
using UnityEngine;

public struct SaveExternalDataCommand
{
    public async UniTask<bool> ExecuteAsync(UserModel userModel)
    {
        Debug.Log("---SaveExternalDataCommand: " + userModel.Uid);
#if UNITY_EDITOR
        if (DebugDataHolder.Instance.IsSaveDisabled == true) return true;
#endif

        var dataToSave = GetExportData(userModel.ExternalActionsModel);
        var dataToSaveStr = JsonConvert.SerializeObject(dataToSave);
        var url = string.Format(Urls.SaveExternalDataURL, userModel.Uid);

        var resultOperation = await new WebRequestsSender().PostAsync<CommonResponseDto>(url, dataToSaveStr);

        if (resultOperation.IsSuccess)
        {
            var response = JsonConvert.DeserializeObject<BoolSuccessResponseDto>(resultOperation.Result.response);
            return response.success;
        }
        return resultOperation.IsSuccess;
    }

    private Dictionary<string, object> GetExportData(ExternalActionsModel externalActionsModel)
    {
        var result = new Dictionary<string, object>();

        var dataExporter = DataExporter.Instance;
        result[Constants.FieldExternalActions] = dataExporter.ExportExternalActions(externalActionsModel);

        return result;
    }
}
