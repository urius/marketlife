using System;
using UnityEngine;

namespace Src.View.UI
{
    public class RootCanvasProvider
    {
        public static RootCanvasProvider Instance => _instance.Value;
        private static Lazy<RootCanvasProvider> _instance = new Lazy<RootCanvasProvider>();

        public RectTransform RootCanvasTransform { get; private set; }

        public void SetupRootCanvas(Canvas rootCanvas)
        {
            RootCanvasTransform = rootCanvas.transform as RectTransform;
        }
    }
}
