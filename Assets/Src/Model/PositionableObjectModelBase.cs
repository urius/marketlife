using System;
using Src.Common;
using Src.Model.Common;
using UnityEngine;

namespace Src.Model
{
    public class PositionableObjectModelBase
    {
        public event Action<int, int> SideChanged = delegate { };
        public event Action<PositionableObjectModelBase, Vector2Int, Vector2Int> CoordsChanged = delegate { };

        public readonly bool TwoSidesMode;

        private readonly int[][] _defaultBuildMatrix;

        private Vector2Int _coords;
        private int _side = -1;

        public PositionableObjectModelBase(int[][] matrix, bool twoSidesMode, Vector2Int coords, int side)
        {
            _defaultBuildMatrix = matrix;

            Side = side;
            Coords = coords;

            TwoSidesMode = twoSidesMode;
        }

        public int Angle => SideHelper.ConvertSideToAngle(_side);
        public int Side
        {
            get { return _side; }
            set
            {
                if (value == _side) return;
                var sideBefore = _side;
                _side = SideHelper.ClampSide(value, TwoSidesMode);
                RotatedBuildMatrix = _defaultBuildMatrix.Rotate(_side - 3);
                SideChanged(sideBefore, _side);
            }
        }

        public Vector2Int Coords
        {
            get { return _coords; }
            set
            {
                if (value == _coords) return;
                var coordsBefore = _coords;
                _coords = value;
                CoordsChanged(this, coordsBefore, value);
            }
        }
        public int[][] RotatedBuildMatrix { get; private set; }
    }
}
