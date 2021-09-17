using Newtonsoft.Json;
using UnityEngine;

public struct ProcessJsMessageCommand
{
    private const string SetVkPlatfomDataCommand = "SetVkPlatformData";
    private const string SetVkFriendsDataCommand = "SetVkFriendsData";
    private const string BuyVkMoneyResultCommand = "BuyVkMoneyResult";

    public void Execute(string message)
    {
        var deserialized = JsonConvert.DeserializeObject<JsCommonCommandDto>(message);
        Debug.Log($"Incoming Js command {message}");
        switch (deserialized.command)
        {
            case SetVkPlatfomDataCommand:
                new SetVkPlatformDataCommand().Execute(message);
                break;
            case SetVkFriendsDataCommand:
                new SetVkFriendsDataCommand().Execute(message);
                break;
            case BuyVkMoneyResultCommand:
                new ProcessBuyVkMoneyResultCommand().Execute(message);
                break;
        }
    }
}

public struct JsCommonCommandDto
{
    public string command;
}
