using System.Runtime.InteropServices;
using Newtonsoft.Json;
using UnityEngine;

public class JsBridge : MonoBehaviour
{
    private const string SetVkPlatfomDataCommand = "SetVkPlatformData";
    private const string SetVkFriendsDataCommand = "SetVkFriendsData";
    private const string InviteVkFriendCommand = "InviteFriend";

    private Dispatcher _dispatcher;

    [DllImport("__Internal")]
    private static extern void SendToJs(string str);

    public void Awake()
    {
        Debug.Log("JsBridge Awaken");

        _dispatcher = Dispatcher.Instance;

        Activate();
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
            case SetVkFriendsDataCommand:
                new SetVkFriendsDataCommand().Execute(payload);
                break;
        }
    }

    public void SendCommandToJs(string command, object payload)
    {
        Debug.Log("Unity->Js: command = " + command);

        var dto = new UnityToJsCommonCommandDto()
        {
            command = command,
            payload = payload,
        };
        var dtoStr = JsonConvert.SerializeObject(dto);
        SendToJs(dtoStr);
    }

    private void Activate()
    {
        _dispatcher.UIBottomPanelInviteFriendClicked += OnUIBottomPanelInviteFriendClicked;
    }

    private void OnUIBottomPanelInviteFriendClicked(FriendData friendData)
    {
        SendCommandToJs(InviteVkFriendCommand, new InviteVkFriendPayload() { uid = friendData.Uid });
    }
}

public struct JsCommonCommandDto
{
    public string command;
}

public struct UnityToJsCommonCommandDto
{
    public string command;
    public object payload;
}

public struct InviteVkFriendPayload
{
    public string uid;
}
