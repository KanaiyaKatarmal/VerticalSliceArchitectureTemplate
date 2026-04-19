using VerticalSliceArchitectureTemplate.Abstractions;

namespace VerticalSliceArchitectureTemplate.Extensions;

public sealed class EventDispatcher(IServiceProvider serviceProvider) : IEventDispatcher
{
    public async Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IDomainEvent
    {
        var handlers = serviceProvider.GetServices<IEventHandler<TEvent>>();
        foreach (var handler in handlers)
            await handler.HandleAsync(@event, cancellationToken);
    }
}
