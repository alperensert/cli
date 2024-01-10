namespace Commands;

public interface ICliCommand
{
    void OnExecute();

    Task OnExecuteAsync(CancellationToken cancellationToken);
}