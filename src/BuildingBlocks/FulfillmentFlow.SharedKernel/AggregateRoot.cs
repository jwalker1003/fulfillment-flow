namespace FulfillmentFlow.SharedKernel;

/// <summary>
/// The entry point to a consistency boundary. Records the domain events raised by its methods
/// and carries a <see cref="Version"/> for optimistic concurrency.
/// </summary>
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot(TId id)
        : base(id)
    {
    }

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public int Version { get; private set; }

    public void ClearDomainEvents() => _domainEvents.Clear();

    /// <summary>Hook for persistence to bump the version after a successful save.</summary>
    internal void IncrementVersion() => Version++;

    protected void Raise(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        _domainEvents.Add(domainEvent);
    }
}
