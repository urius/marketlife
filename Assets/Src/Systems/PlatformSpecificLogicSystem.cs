using System;
using Newtonsoft.Json;
using UnityEngine;

public class PlatformSpecificLogicSystem
{
    private readonly Dispatcher _dispatcher;
    //
    private PlatformSpecificLogicModuleBase _module;

    public PlatformSpecificLogicSystem()
    {
        _dispatcher = Dispatcher.Instance;
    }

    public void Start()
    {
        Activate();
    }

    private void Activate()
    {
        _dispatcher.JsIncomingMessage += OnJsIncomingMessage;
    }

    private void OnJsIncomingMessage(string message)
    {
        var deserialized = JsonConvert.DeserializeObject<JsCommonCommandDto>(message);
        Debug.Log($"Incoming Js command {message}");
        switch (deserialized.command)
        {
            case "SetVkPlatformData":
                InitModuleVK();
                new SetVkPlatformDataCommand().Execute(message);
                break;
        }
    }

    private void InitModuleVK()
    {
        _module = new VKLogicModule();
        _module.Start();
    }
}

public abstract class PlatformSpecificLogicModuleBase
{
    public abstract void Start();
}

public class VKLogicModule : PlatformSpecificLogicModuleBase
{
    private readonly GameStateModel _gameStateModel;
    private readonly PlayerModelHolder _playerModelHolder;
    private readonly JsBridge _jsBridge;
    private readonly Dispatcher _dispatcher;

    public VKLogicModule()
    {
        _gameStateModel = GameStateModel.Instance;
        _playerModelHolder = PlayerModelHolder.Instance;
        _jsBridge = JsBridge.Instance;
        _dispatcher = Dispatcher.Instance;
    }

    public override async void Start()
    {
        _dispatcher.JsIncomingMessage += OnJsIncomingMessage;
        _dispatcher.UIBankItemClicked += OnUIBankItemClicked;
        _dispatcher.UIBottomPanelInviteFriendClicked += OnUIBottomPanelInviteFriendClicked;

        await _gameStateModel.GameDataLoadedTask;

        _playerModelHolder.UserModel.ProgressModel.LevelChanged += OnLevelChanged;
    }

    private void OnJsIncomingMessage(string message)
    {
        var deserialized = JsonConvert.DeserializeObject<JsCommonCommandDto>(message);
        switch (deserialized.command)
        {
            case "SetVkFriendsData":
                new SetVkFriendsDataCommand().Execute(message);
                break;
            case "BuyVkMoneyResult":
                new ProcessBuyVkMoneyResultCommand().Execute(message);
                break;
        }
    }

    private void OnUIBottomPanelInviteFriendClicked(FriendData friendData)
    {
        AnalyticsManager.Instance.SendCustom(AnalyticsManager.EventNameInviteFriendClicked);
        JsBridge.Instance.SendCommandToJs("InviteFriend", new InviteVkFriendPayload() { uid = friendData.Uid });
    }

    private void OnLevelChanged(int delta)
    {
        var newLevel = _playerModelHolder.UserModel.ProgressModel.Level;
        _jsBridge.SendCommandToJs("LevelUp", new LevelUpJsPayload(newLevel));
    }

    private void OnUIBankItemClicked(BankConfigItem itemConfig)
    {
        _gameStateModel.ChargedBankItem = itemConfig;
        JsBridge.Instance.SendCommandToJs("BuyMoney", new BuyVkMoneyPayload(itemConfig.Id));
    }
}

public struct JsCommonCommandDto
{
    public string command;
}

public struct LevelUpJsPayload
{
    public int level;

    public LevelUpJsPayload(int value)
    {
        level = value;
    }
}

public struct BuyVkMoneyPayload
{
    public string product;

    public BuyVkMoneyPayload(string id)
    {
        product = id;
    }
}

public struct InviteVkFriendPayload
{
    public string uid;
}