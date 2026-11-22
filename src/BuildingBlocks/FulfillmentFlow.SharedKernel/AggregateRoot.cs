namespace FulfillmentFlow.SharedKernel;

public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    protected AggregateRoot(TId id)
        : base(id)
    {
    }

    public IReadOnlyList<IDomainEvent> DomainEvents => throw new NotImplementedException();

    public int Version => throw new NotImplementedException();

    public void ClearDomainEvents() => throw new NotImplementedException();

    internal void IncrementVersion() => throw new NotImplementedException();

    protected void Raise(IDomainEvent domainEvent) => throw new NotImplementedException();
}
