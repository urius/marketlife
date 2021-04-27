using System;
using UnityEngine;

public class EventsHandleSystem : MonoBehaviour
{
    public void Initialize()
    {
        var dispatcher = Dispatcher.Instance;

        dispatcher.UIBottomPanelPlaceShelfClicked += OnUIBottomPanelPlaceShelfClicked;
    }

    private void OnUIBottomPanelPlaceShelfClicked(int shelfId)
    {
        new UIRequestPlaceShelfCommand().Execute(shelfId);
    }
}
