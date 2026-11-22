namespace FulfillmentFlow.SharedKernel;

public abstract class Entity<TId>
    where TId : notnull
{
    protected Entity(TId id) => throw new NotImplementedException();

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) => throw new NotImplementedException();

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => throw new NotImplementedException();

    public override bool Equals(object? obj) => throw new NotImplementedException();

    public override int GetHashCode() => throw new NotImplementedException();
}
