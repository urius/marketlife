using System;
using UnityEngine;
using UnityEngine.UI;

public class UIDailyMIssionsButtonView : MonoBehaviour
{
    public event Action OnClick = delegate { };

    [SerializeField] private Button _button;
    [SerializeField] private Image _notificationPointImage;

    public void SetNotificationVisibility(bool isVisible)
    {
        _notificationPointImage.gameObject.SetActive(isVisible);
    }

    public void SetNotificationColor(Color color)
    {
        _notificationPointImage.color = color;
    }

    private void Awake()
    {
        SetNotificationVisibility(false);
        _button.AddOnClickListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        OnClick();
    }
}
