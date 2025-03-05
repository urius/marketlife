using System;

namespace Src.Debug_Scripts
{
    public class DebugDispatcher
    {
        public static DebugDispatcher Instance => _instance ?? CreateAndGetInstance();

        private static DebugDispatcher _instance;

        public Action<int> ShowHumanSide = delegate { };
        public Action<int> ShowHumanAnimation = delegate { };
        public Action<int> ShowFaceAnimation = delegate { };
        public Action<int> ShowHair = delegate { };
        public Action<int> ShowTopClothes = delegate { };
        public Action<int> ShowBottomClothes = delegate { };
        public Action<int> ShowGlasses = delegate { };

        private static DebugDispatcher CreateAndGetInstance()
        {
            _instance = new DebugDispatcher();
            return _instance;
        }
    }
}
