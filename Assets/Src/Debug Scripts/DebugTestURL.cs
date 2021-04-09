using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class DebugTestURL : MonoBehaviour
{
    [SerializeField] private string _id;

    private string GetDataUrl => string.Format("https://devman.ru/marketVK/dataProvider.php?command=get_data&id={0}", _id);

    public async UniTaskVoid Load()
    {
        var resultOperation = await new WebRequestsSender().GetAsync(GetDataUrl);
        if (resultOperation.IsSuccess)
        {
            var result = JsonConvert.DeserializeObject<GetDataResponseDto>(resultOperation.Result);

            result.ToShopModel();

            Debug.Log(result);
        }
    }
}
