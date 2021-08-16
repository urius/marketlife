using System.Runtime.InteropServices;
using Newtonsoft.Json;
using UnityEngine;

public class JsBridge : MonoBehaviour
{
    private const string SetVkPlatfomDataCommand = "SetVkPlatformData";

    [DllImport("__Internal")]
    private static extern void Hello();

    [DllImport("__Internal")]
    private static extern void HelloString(string str);

    public void Awake()
    {
        Debug.Log("JsBridge Awaken");
    }

    public void JsStr1(string str)
    {
        Debug.Log("TestFunction str:" + str);

        HelloString(str);
    }

    public void JsCommandMessage(string payload)
    {
        var deserialized = JsonConvert.DeserializeObject<JsCommonCommandDto>(payload);
        Debug.Log($"Incoming Js command {payload}");
        switch (deserialized.command)
        {
            case SetVkPlatfomDataCommand:
                new SetVkPlatformDataCommand().Execute(payload);
                break;
        }
    }
}

public struct JsCommonCommandDto
{
    public string command;
}

public struct JsVkPlatfomDataCommandDto
{
    public string command;
    public JsVkPlatfomDataCommandDataDto data;
}

public struct JsVkPlatfomDataCommandDataDto
{
    public string viewer_id;
}
