using System;
using Cysharp.Threading.Tasks;

public static class UniTaskExtensions
{
    public static UniTask ToUniTask(this Action @event)
    {
        var uniTaskTcs = new UniTaskCompletionSource();
        void OnEvent()
        {
            uniTaskTcs.TrySetResult();
            @event -= OnEvent;
        }
        @event += OnEvent;

        return uniTaskTcs.Task;
    }

    public static UniTask<T> ToUniTask<T>(this Action<T> @event)
    {
        var uniTaskTcs = new UniTaskCompletionSource<T>();
        void OnEvent(T param)
        {
            uniTaskTcs.TrySetResult(param);
            @event -= OnEvent;
        }
        @event += OnEvent;

        return uniTaskTcs.Task;
    }

    public static UniTask<(T1, T2)> ToUniTask<T1, T2>(this Action<T1, T2> @event)
    {
        var uniTaskTcs = new UniTaskCompletionSource<(T1, T2)>();
        void OnEvent(T1 param1, T2 param2)
        {
            uniTaskTcs.TrySetResult((param1, param2));
            @event -= OnEvent;
        }
        @event += OnEvent;

        return uniTaskTcs.Task;
    }

    public static UniTask<(T1, T2, T3)> ToUniTask<T1, T2, T3>(this Action<T1, T2, T3> @event)
    {
        var uniTaskTcs = new UniTaskCompletionSource<(T1, T2, T3)>();
        void OnEvent(T1 param1, T2 param2, T3 param3)
        {
            uniTaskTcs.TrySetResult((param1, param2, param3));
            @event -= OnEvent;
        }
        @event += OnEvent;

        return uniTaskTcs.Task;
    }

    public static UniTask<(T1, T2, T3, T4)> ToUniTask<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> @event)
    {
        var uniTaskTcs = new UniTaskCompletionSource<(T1, T2, T3, T4)>();
        void OnEvent(T1 param1, T2 param2, T3 param3, T4 param4)
        {
            uniTaskTcs.TrySetResult((param1, param2, param3, param4));
            @event -= OnEvent;
        }
        @event += OnEvent;

        return uniTaskTcs.Task;
    }


    public static UniTask<(T1, T2, T3, T4, T5)> ToUniTask<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> @event)
    {
        var uniTaskTcs = new UniTaskCompletionSource<(T1, T2, T3, T4, T5)>();
        void OnEvent(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            uniTaskTcs.TrySetResult((param1, param2, param3, param4, param5));
            @event -= OnEvent;
        }
        @event += OnEvent;

        return uniTaskTcs.Task;
    }
}
