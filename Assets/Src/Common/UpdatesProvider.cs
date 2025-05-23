using System;

namespace Src.Common
{
    public class UpdatesProvider
    {
        private static Lazy<UpdatesProvider> _instance = new Lazy<UpdatesProvider>();
        public static UpdatesProvider Instance => _instance.Value;

        public event Action RealtimeUpdate = delegate { };
        public event Action GametimeUpdate = delegate { };
        public event Action RealtimeSecondUpdate = delegate { };
        public event Action GameplaySecondUpdate = delegate { };
        public event Action GameplayHalfSecondPassed = delegate { };
        public event Action GameplayQuarterSecondPassed = delegate { };

        public void CallRealtimeUpdate()
        {
            RealtimeUpdate();
        }

        public void CallGameplayUpdate()
        {
            GametimeUpdate();
        }

        public void CallRealtimeSecondUpdate()
        {
            RealtimeSecondUpdate();
        }

        public void CallGameplaySecondUpdate()
        {
            GameplaySecondUpdate();
        }

        public void CallGameplayHalfSecondPassed()
        {
            GameplayHalfSecondPassed();
        }

        public void CallGameplayQuarterSecondPassed()
        {
            GameplayQuarterSecondPassed();
        }
    }
}
