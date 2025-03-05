using System;

namespace Src.Common_Utils
{
    public struct DisposableSource : IDisposable
    {
        private readonly Action _disposeAction;

        public DisposableSource(Action disposeAction)
        {
            _disposeAction = disposeAction;
        }

        public void Dispose()
        {
            _disposeAction?.Invoke();
        }
    }
}
