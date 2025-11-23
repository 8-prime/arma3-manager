namespace ArmA3Manager.Application.Common.Models.Server;

public record UpdateOperation(Guid Id, Task Operation, CancellationTokenSource CancellationTokenSource);