using Cysharp.Threading.Tasks;

namespace Src.Commands
{
    public interface IAsyncCommand
    {
        UniTask ExecuteAsync();
    }

    public interface IAsyncGameLoadCommand
    {
        UniTask<bool> ExecuteAsync();
    }
}