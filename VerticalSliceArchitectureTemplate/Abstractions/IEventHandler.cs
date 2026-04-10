namespace VerticalSliceArchitectureTemplate.Abstractions;

public interface IEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
}
