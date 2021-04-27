
public class BottomPanelMediator : UINotMonoMediatorBase<BottomPanelView>
{
    private UINotMonoMediatorBase<BottomPanelView> _currentTabMediator;

    protected override void OnStart()
    {
        _currentTabMediator = new UIBottomPanelShelfsTabMediator();
        _currentTabMediator.Mediate(View);
    }

    protected override void OnStop()
    {

    }
}
