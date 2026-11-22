namespace FulfillmentFlow.SharedKernel;

public interface IDomainEvent
{
    DateTimeOffset OccurredAt { get; }
}
