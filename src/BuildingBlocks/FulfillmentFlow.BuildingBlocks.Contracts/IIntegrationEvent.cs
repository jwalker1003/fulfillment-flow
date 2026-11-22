namespace FulfillmentFlow.BuildingBlocks.Contracts;

public interface IIntegrationEvent
{
    Guid MessageId { get; }

    DateTimeOffset OccurredAt { get; }
}
