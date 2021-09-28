using Cysharp.Threading.Tasks;

public interface IAsyncCommand
{
    UniTask ExecuteAsync();
}

public interface IAsyncGameLoadCommand
{
    UniTask<bool> ExecuteAsync();
}
