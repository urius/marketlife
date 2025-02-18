using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Src.Common;
using UnityEngine;

public class DebugTestURL : MonoBehaviour
{
    [SerializeField] private string _id;

    private string GetDataUrl => string.Format($"{Urls.BaseHostUrl}/marketVK/dataProvider.php?command=get_data&id={0}", _id);

    public async UniTaskVoid Load()
    {
        var resultOperation = await new WebRequestsSender().GetAsync(GetDataUrl);
        if (resultOperation.IsSuccess)
        {
            var result = JsonConvert.DeserializeObject<GetDataOldResponseDto>(resultOperation.Result);

            DataImporter.Instance.ImportOld(result);

            Debug.Log(result);
        }
    }
}
