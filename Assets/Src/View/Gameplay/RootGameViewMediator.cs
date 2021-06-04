using System;
using UnityEngine;

public class RootGameViewMediator : MonoBehaviour
{
    [SerializeField] private Grid _grid;

    private UpdatesProvider _updatesProvider;
    private GridCalculator _gridCalculator;
    private int NextSecondUpdate;

    private void Awake()
    {
        _updatesProvider = UpdatesProvider.Instance;

        SetupGridCalculator();
        NextSecondUpdate = (int)Time.realtimeSinceStartup + 1;
    }

    private void FixedUpdate()
    {
        _updatesProvider.CallGameplayUpdate();
        _updatesProvider.CallRealtimeUpdate();
        if (Time.realtimeSinceStartup >= NextSecondUpdate)
        {
            NextSecondUpdate = (int)Time.realtimeSinceStartup + 1;
            _updatesProvider.CallRealtimeSecondUpdate();
        }
    }

    private void SetupGridCalculator()
    {
        _gridCalculator = GridCalculator.Instance;
        _gridCalculator.SetGrid(_grid);
    }

    private void OnDrawGizmos()
    {
        if (_gridCalculator == null)
        {
            SetupGridCalculator();
        }

        Gizmos.DrawSphere(_gridCalculator.CellToWord(new Vector2Int(0, 0)), 0.5f);
        Gizmos.DrawSphere(_gridCalculator.CellToWord(new Vector2Int(1, 0)), 0.5f);
        Gizmos.DrawSphere(_gridCalculator.CellToWord(new Vector2Int(2, 0)), 0.5f);
        Gizmos.DrawSphere(_gridCalculator.CellToWord(new Vector2Int(3, 0)), 0.5f);
        Gizmos.DrawSphere(_gridCalculator.CellToWord(new Vector2Int(0, 1)), 0.5f);
        Gizmos.DrawSphere(_gridCalculator.GetCellRightCorner(new Vector2Int(0, 1)), 0.5f);
    }
}
