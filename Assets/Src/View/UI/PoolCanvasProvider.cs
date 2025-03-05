using System;
using UnityEngine;

namespace Src.View.UI
{
    public class PoolCanvasProvider
    {
        public static PoolCanvasProvider Instance => _instance.Value;
        private static Lazy<PoolCanvasProvider> _instance = new Lazy<PoolCanvasProvider>();

        public Transform PoolCanvasTransform { get; private set; }

        public void SetupPoolCanvas(Canvas poolCanvas)
        {
            PoolCanvasTransform = poolCanvas.transform;
        }
    }
}
