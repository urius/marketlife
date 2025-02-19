using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Src.Common;
using UnityEngine;

public struct SaveDataCommand
{
    public async UniTask<bool> ExecuteAsync(SaveField saveFields)
    {
#if UNITY_EDITOR
        if (DebugDataHolder.Instance.IsSaveDisabled == true) return true;
#endif
        if (saveFields == SaveField.None) return true;

        Debug.Log("---SaveDataCommand: " + saveFields);

        var playerModel = PlayerModelHolder.Instance.UserModel;
        var url = string.Format(Urls.SaveDataURL, playerModel.Uid);
        var dataToSave = GetExportData(saveFields, playerModel);
        var dataToSaveStr = JsonConvert.SerializeObject(dataToSave);

        var resultOperation = await new WebRequestsSender().PostAsync<CommonResponseDto>(url, dataToSaveStr);

        if (resultOperation.IsSuccess)
        {
            var response = JsonConvert.DeserializeObject<BoolSuccessResponseDto>(resultOperation.Result.response);
            return response.success;
        }
        return resultOperation.IsSuccess;
    }

    private Dictionary<string, object> GetExportData(SaveField saveFields, UserModel userModel)
    {
        var dataExporter = DataExporter.Instance;
        var shopModel = userModel.ShopModel;

        var result = new Dictionary<string, object>();
        if (CheckSaveField(saveFields, SaveField.Progress))
        {
            result[Constants.FieldProgress] = dataExporter.ExportProgress(userModel.ProgressModel);
        }

        if (CheckSaveField(saveFields, SaveField.ShopObjects))
        {
            result[Constants.FieldShopObjects] = dataExporter.ExportShopObjects(shopModel);
        }

        if (CheckSaveField(saveFields, SaveField.Warehouse))
        {
            result[Constants.FieldWarehouse] = dataExporter.ExportWarehouse(shopModel);
        }

        if (CheckSaveField(saveFields, SaveField.Personal))
        {
            result[Constants.FieldPersonal] = dataExporter.ExportPersonal(shopModel);
        }

        if (CheckSaveField(saveFields, SaveField.Design))
        {
            result[Constants.FieldDesign] = dataExporter.ExportDesign(shopModel);
        }

        if (CheckSaveField(saveFields, SaveField.Unwashes))
        {
            result[Constants.FieldUnwashes] = dataExporter.ExportUnwashes(shopModel);
        }

        if (CheckSaveField(saveFields, SaveField.TutorialSteps))
        {
            result[Constants.FieldTutorialSteps] = dataExporter.ExportTutorialSteps(userModel);
        }

        if (CheckSaveField(saveFields, SaveField.FriendsActionsData))
        {
            result[Constants.FieldAvailableActionsData] = dataExporter.ExportAvailableActionsData(userModel);
        }

        if (CheckSaveField(saveFields, SaveField.Bonus))
        {
            result[Constants.FieldBonus] = dataExporter.ExportBonus(userModel.BonusState);
        }

        if (CheckSaveField(saveFields, SaveField.Settings))
        {
            result[Constants.FieldSettings] = dataExporter.ExportSettings(userModel.UserSettingsModel);
        }

        if (CheckSaveField(saveFields, SaveField.Billboard))
        {
            result[Constants.FieldBillboard] = dataExporter.ExportBillboardState(shopModel.BillboardModel);
        }

        if (CheckSaveField(saveFields, SaveField.DailyMissions))
        {
            result[Constants.DailyMissions] = dataExporter.ExportDailyMissions(userModel.DailyMissionsModel);
        }

        return result;
    }

    private bool CheckSaveField(SaveField saveFields, SaveField checkingField)
    {
        return (saveFields & checkingField) == checkingField;
    }
}
