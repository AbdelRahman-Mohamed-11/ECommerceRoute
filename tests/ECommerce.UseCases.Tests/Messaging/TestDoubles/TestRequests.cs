using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Tests.Messaging.TestDoubles;

internal sealed record PingQuery(string Message) : IQuery<string>;

internal sealed class PingQueryHandler : IRequestHandler<PingQuery, string>
{
    public Task<string> Handle(PingQuery request, CancellationToken cancellationToken) =>
        Task.FromResult($"pong:{request.Message}");
}

internal sealed record CancellationProbeQuery : IQuery<bool>;

internal sealed class CancellationProbeQueryHandler : IRequestHandler<CancellationProbeQuery, bool>
{
    public Task<bool> Handle(CancellationProbeQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(cancellationToken.IsCancellationRequested);
}

internal sealed record UnregisteredQuery : IQuery<string>;
