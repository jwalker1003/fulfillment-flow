namespace FulfillmentFlow.SharedKernel;

/// <summary>Something that happened inside an aggregate, named in the past tense from the glossary.</summary>
public interface IDomainEvent
{
    DateTimeOffset OccurredAt { get; }
}
