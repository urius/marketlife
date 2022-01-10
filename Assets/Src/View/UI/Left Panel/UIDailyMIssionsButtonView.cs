using System;
using UnityEngine;
using UnityEngine.UI;

public class UIDailyMIssionsButtonView : MonoBehaviour
{
    public event Action OnClick = delegate { };

    [SerializeField] private Button _button;
    [SerializeField] private Image _notificationPointImage;
    [SerializeField] private Image _notificationCheckImage;

    public void SetNotificationVisibility(bool isVisible)
    {
        _notificationPointImage.gameObject.SetActive(isVisible);
    }

    public void SetNotificationColor(Color color)
    {
        _notificationPointImage.color = color;
    }

    public void SetNotificationCheckVisibility(bool isVisible)
    {
        _notificationCheckImage.gameObject.SetActive(isVisible);
    }

    private void Awake()
    {
        SetNotificationVisibility(false);
        SetNotificationCheckVisibility(false);
        _button.AddOnClickListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        OnClick();
    }
}
