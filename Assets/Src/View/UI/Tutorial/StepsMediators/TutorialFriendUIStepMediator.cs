using System;
using Src.Managers;
using UnityEngine;

public class TutorialFriendUIStepMediator : TutorialStepMediatorBase
{
    private readonly LocalizationManager _loc;
    private readonly TutorialUIElementsProvider _tutorialUIElementsProvider;

    private int _phaseIndex = 0;

    public TutorialFriendUIStepMediator(RectTransform parentTransform)
        : base(parentTransform)
    {
        _loc = LocalizationManager.Instance;
        _tutorialUIElementsProvider = TutorialUIElementsProvider.Instance;
    }

    public override void Mediate()
    {
        base.Mediate();
        View.SetButtonText(_loc.GetLocalization($"{LocalizationKeys.TutorialButtonPrefix}{ViewModel.StepIndex}"));
    }

    protected override void OnViewButtonClicked()
    {
        _phaseIndex++;
        switch (_phaseIndex)
        {
            case 1:
                HandleTakePhase();
                break;
            case 2:
                HandleAddUnwashPhase();
                break;
            case 3:
                HandleAddProductPhase();
                break;
            default:
                base.OnViewButtonClicked();
                break;
        }
    }

    private void HandleTakePhase()
    {
        var (message, buttonText) = GetTextsForCurrentPhase();
        View.SetMessageText(message);
        View.SetButtonText(buttonText);
        HighlightUIElement(TutorialUIElement.BottomPanelFriendShopTakeButton);
    }

    private void HandleAddUnwashPhase()
    {
        var (message, buttonText) = GetTextsForCurrentPhase();
        View.SetMessageText(message);
        View.SetButtonText(buttonText);
        HighlightUIElement(TutorialUIElement.BottomPanelFriendShopAddUnwashButton);
    }

    private void HandleAddProductPhase()
    {
        var (message, buttonText) = GetTextsForCurrentPhase();
        View.SetMessageText(message);
        View.SetButtonText(buttonText);
        HighlightUIElement(TutorialUIElement.BottomPanelFriendShopAddProductButton);
    }

    private (string message, string buttonText) GetTextsForCurrentPhase()
    {
        var message = _loc.GetLocalization($"{LocalizationKeys.TutorialMessagePrefix}{ViewModel.StepIndex}_phase_{_phaseIndex}");
        var buttonText = _loc.GetLocalization($"{LocalizationKeys.TutorialButtonPrefix}{ViewModel.StepIndex}_phase_{_phaseIndex}");
        return (message, buttonText);
    }

    private void HighlightUIElement(TutorialUIElement elementType)
    {
        var rectTransform = _tutorialUIElementsProvider.GetElementRectTransform(elementType);
        var sideSize = 1.4f * Math.Max(rectTransform.rect.size.x, rectTransform.rect.size.y);
        View.HighlightScreenRoundArea(rectTransform.position, new Vector2(sideSize, sideSize), animated: true);
    }
}
