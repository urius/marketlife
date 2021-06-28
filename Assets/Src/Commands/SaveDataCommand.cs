using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public struct SaveDataCommand
{
    public async UniTask<bool> Execute(SaveField saveFields)
    {
        if (saveFields == SaveField.None) return true;

        Debug.Log("---SaveDataCommand: " + saveFields);

        var playerModel = PlayerModel.Instance;
        var shopModel = playerModel.ShopModel;
        var url = string.Format(URLsHolder.Instance.SaveDataURL, playerModel.Uid);

        var dataToSave = GetExportData(saveFields, shopModel);
        var dataToSaveStr = JsonConvert.SerializeObject(dataToSave);

        var resultOperation = await new WebRequestsSender().PostAsync<CommonResponseDto>(url, dataToSaveStr);

        if (resultOperation.IsSuccess)
        {
            var response = JsonConvert.DeserializeObject<BoolSuccessResponseDto>(resultOperation.Result.response);
            return response.success;
        }
        return resultOperation.IsSuccess;
    }

    private Dictionary<string, object> GetExportData(SaveField saveFields, ShopModel shopModel)
    {
        var dataExporter = DataExporter.Instance;

        var result = new Dictionary<string, object>();
        if (CheckSaveField(saveFields, SaveField.Progress))
        {
            result[Constants.FieldProgress] = dataExporter.ExportProgress(shopModel);
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

        return result;
    }

    private bool CheckSaveField(SaveField saveFields, SaveField checkingField)
    {
        return (saveFields & checkingField) == checkingField;
    }
}
