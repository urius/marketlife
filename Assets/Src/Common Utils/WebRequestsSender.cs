using System;
using System.Security.Cryptography;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Src.Common_Utils
{
    public class WebRequestsSender
    {
        private int[] _retryMsArray = new int[] { 0, 500, 1000, 5000 };

        public async UniTask<WebRequestResult<T>> GetAsync<T>(string url)
        {
            var resultStr = await GetAsync(url);
            return HandleGenericResponse<T>(resultStr);
        }

        public UniTask<WebRequestResult<string>> GetAsync(string url)
        {
            return SendRequestAsync(() => UnityWebRequest.Get(url));
        }

        public async UniTask<WebRequestResult<T>> PostAsync<T>(string url, string postData)
        {
            var resultStr = await PostAsync(url, postData);
            return HandleGenericResponse<T>(resultStr);
        }

        public UniTask<WebRequestResult<string>> PostAsync(string url, string postData)
        {
            UnityWebRequest factory()
            {
                WWWForm form = new WWWForm();
                form.AddField("data", postData);
                form.AddField("hash", MD5Hash(postData));
                return UnityWebRequest.Post(url, form);
            }

            return SendRequestAsync(factory);
        }

        private async UniTask<WebRequestResult<string>> SendRequestAsync(Func<UnityWebRequest> unityWebRequestFactory)
        {
            var retryPrefix = string.Empty;
            for (var i = 0; i < _retryMsArray.Length; i++)
            {
                var unityWebRequest = unityWebRequestFactory();
                using (unityWebRequest)
                {
                    if (_retryMsArray[i] > 0)
                    {
                        await UniTask.Delay(_retryMsArray[i]);
                        retryPrefix = $"[ Retry #{i} ]";
                    }

                    try
                    {
                        Debug.Log($"{retryPrefix}[ Request ] -> {unityWebRequest.url}\n");
                        var result = await unityWebRequest.SendWebRequest();

                        Debug.Log($"[ Response ] -> {result.downloadHandler.text} (url: {unityWebRequest.url})\n");
                        if (result.error == null)
                        {
                            return new WebRequestResult<string>(result.downloadHandler.text);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }

            return new WebRequestResult<string>();
        }

        private WebRequestResult<T> HandleGenericResponse<T>(WebRequestResult<string> resultStr)
        {
            if (resultStr.IsSuccess)
            {
                T result;
                try
                {
                    result = JsonUtility.FromJson<T>(resultStr.Result);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return new WebRequestResult<T>();
                }
                return new WebRequestResult<T>(result);
            }
            else
            {
                return new WebRequestResult<T>();
            }
        }

        private static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }
    }

    public struct WebRequestResult<T>
    {
        public WebRequestResult(T result)
        {
            IsSuccess = true;
            Result = result;
        }

        public bool IsSuccess;
        public T Result;
    }
}