using System;

public class BottomPanelViewModel
{
    public event Action SimulationTabChanged = delegate { };
    public event Action InteriorTabChanged = delegate { };

    public BottomPanelSimulationModeTab SimulationModeTab { get; private set; } = BottomPanelSimulationModeTab.Warehouse;
    public BottomPanelInteriorModeTab InteriorModeTab { get; private set; } = BottomPanelInteriorModeTab.Shelfs;

    public void SetSimulationTab(BottomPanelSimulationModeTab tab)
    {
        SimulationModeTab = tab;
        SimulationTabChanged();
    }

    public void SetInteriorTab(BottomPanelInteriorModeTab tab)
    {
        InteriorModeTab = tab;
        InteriorTabChanged();
    }
}

public enum BottomPanelSimulationModeTab
{
    None,
    Friends,
    Warehouse,
}

public enum BottomPanelInteriorModeTab
{
    None,
    Shelfs,
    Floors,
    Walls,
    Windows,
    Doors,
}
