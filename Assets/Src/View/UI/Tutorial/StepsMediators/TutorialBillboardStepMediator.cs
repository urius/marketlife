using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TutorialBillboardStepMediator : TutorialStepMediatorBase
{
    private readonly Dispatcher _dispatcher;
    private readonly TutorialUIElementsProvider _tutorialUIElementsProvider;
    private readonly UpdatesProvider _updatesProvider;
    private readonly ScreenCalculator _screenCalculator;
    private readonly LocalizationManager _loc;

    public TutorialBillboardStepMediator(RectTransform parentTransform)
        : base(parentTransform)
    {
        _dispatcher = Dispatcher.Instance;
        _tutorialUIElementsProvider = TutorialUIElementsProvider.Instance;
        _updatesProvider = UpdatesProvider.Instance;
        _screenCalculator = ScreenCalculator.Instance;
        _loc = LocalizationManager.Instance;
    }

    public override void Mediate()
    {
        base.Mediate();

        View.SetButtonText(_loc.GetLocalization($"{LocalizationKeys.TutorialButtonPrefix}{ViewModel.StepIndex}"));
        View.SetQuadrant(2);

        Activate();
    }

    public override void Unmediate()
    {
        Deactivate();

        base.Unmediate();
    }

    private void Activate()
    {
        _updatesProvider.RealtimeUpdate += OnRealtimeUpdate;
    }

    private void Deactivate()
    {
        _updatesProvider.RealtimeUpdate -= OnRealtimeUpdate;
    }

    private void OnRealtimeUpdate()
    {
        if (_tutorialUIElementsProvider.HasElement(TutorialUIElement.BillboardTransform))
        {
            _updatesProvider.RealtimeUpdate -= OnRealtimeUpdate;
            StartHighlighting();
        }
    }

    private async void StartHighlighting()
    {
        var moveDurationSec = 0.8f;
        var billboardTransform = _tutorialUIElementsProvider.GetElementTransform(TutorialUIElement.BillboardTransform);
        _dispatcher.UIRequestMoveCamera(billboardTransform.position, moveDurationSec);

        var billboardView = billboardTransform.GetComponent<BillboardView>();
        if (billboardView != null)
        {
            await UniTask.Delay((int)(moveDurationSec * 1000));

            var billboardBoundWorldPoints = billboardView.BoundPointTransforms
                .Select(t => t.position)
                .ToArray();
            var billboardBoundsScreenPoints = billboardBoundWorldPoints
                .Select(p => _screenCalculator.WorldToScreenPoint(p))
                .ToArray();
            var center = 0.5f * (billboardBoundWorldPoints[1] + billboardBoundWorldPoints[3]);
            var diameter = 2 * Math.Abs(Vector3.Distance(billboardBoundsScreenPoints[1], billboardBoundsScreenPoints[2]));
            View.HighlightScreenRoundArea(center, new Vector2(diameter, diameter), animated: true);
        }
    }
}
