using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Src
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private string _adressableSpriteAtlasName;
        [SerializeField] private Image _testImage;

        async void Start()
        {
            using (UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle("http://shoplife.01sh.ru/AssetBundles/graphicsgameplay", 1, 0))
            {
                var webRequestResult = await webRequest.SendWebRequest();
                var assetBundle = DownloadHandlerAssetBundle.GetContent(webRequestResult);

                var spriteAtlas = await assetBundle.LoadAssetAsync<AssetBundleManifest>("GameplayGraphicsAtlas") as SpriteAtlas;
            }
        }
    }
}
