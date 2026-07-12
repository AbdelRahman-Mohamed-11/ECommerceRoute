using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Tests.Messaging.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace ECommerce.UseCases.Tests.Messaging;

public sealed class SenderTests
{
    [Fact]
    public async Task Send_dispatches_request_to_registered_handler()
    {
        var sender = CreateSender(typeof(PingQueryHandler).Assembly);

        var response = await sender.Send(new PingQuery("hello"));

        response.ShouldBe("pong:hello");
    }

    [Fact]
    public async Task Send_passes_cancellation_token_to_handler()
    {
        var sender = CreateSender(typeof(CancellationProbeQueryHandler).Assembly);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var response = await sender.Send(new CancellationProbeQuery(), cts.Token);

        response.ShouldBeTrue();
    }

    [Fact]
    public async Task Send_throws_when_no_handler_is_registered()
    {
        var sender = CreateSender(typeof(PingQueryHandler).Assembly);

        var act = () => sender.Send(new UnregisteredQuery());

        var exception = await Should.ThrowAsync<InvalidOperationException>(act);
        exception.Message.ShouldContain(nameof(UnregisteredQuery));
    }

    [Fact]
    public async Task Send_throws_when_request_is_null()
    {
        var sender = CreateSender(typeof(PingQueryHandler).Assembly);

        var act = () => sender.Send<string>(null!);

        await Should.ThrowAsync<ArgumentNullException>(act);
    }

    private static ISender CreateSender(System.Reflection.Assembly assembly)
    {
        var services = new ServiceCollection();
        services.AddMessaging(assembly);

        return services.BuildServiceProvider()
            .GetRequiredService<ISender>();
    }
}
