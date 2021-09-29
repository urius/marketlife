using System.Collections.Generic;
using UnityEngine.Analytics;

public class AnalyticsManager
{
    public static readonly AnalyticsManager Instance = new AnalyticsManager();

    public static string LevelParamName = "level";
    public static string NumFriendsParamName = "friends_count";

    public static string EventNameOrderProduct = "order_product";
    public static string EventNameOfflineProfit = "offline_profit";

    private Dictionary<string, object> _metaParams = new Dictionary<string, object>();

    public void SetupMetaParameter(string name, object value)
    {
        _metaParams[name] = value;
    }

    public void SendGameStart()
    {
        AnalyticsEvent.GameStart(_metaParams);
    }

    public void SendLevelUp(int levelIndex)
    {
        AnalyticsEvent.LevelUp(levelIndex, _metaParams);
    }

    public void SendTutorialStart(string tutorialId)
    {
        AnalyticsEvent.TutorialStart(tutorialId, _metaParams);
    }

    public void SendTutorialStep(int stepIndex, string tutorialId)
    {
        AnalyticsEvent.TutorialStep(stepIndex, tutorialId, _metaParams);
    }

    public void SendStoreOpened(bool isGold)
    {
        AnalyticsEvent.StoreOpened(isGold ? StoreType.Premium : StoreType.Soft, _metaParams);
    }

    public void SendStoreItemClick(bool isGold, string itemId)
    {
        AnalyticsEvent.StoreItemClick(isGold ? StoreType.Premium : StoreType.Soft, itemId, itemId, _metaParams);
    }

    public void SendCustom(string name, params (string Name, object Value)[] parameters)
    {
        var eventData = CollectParams(parameters);
        Analytics.CustomEvent(name, eventData);
    }

    private Dictionary<string, object> CollectParams((string Name, object Value)[] parameters)
    {
        var result = new Dictionary<string, object>();
        foreach (var parameter in parameters)
        {
            result[parameter.Name] = parameter.Value;
        }
        return result;
    }
}
