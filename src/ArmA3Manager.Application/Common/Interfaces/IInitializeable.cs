namespace ArmA3Manager.Application.Common.Interfaces;

public interface IInitializeable
{
    public string Name { get; }
    public Task Initialize();
    public Task OnInitializationCompleted();
}