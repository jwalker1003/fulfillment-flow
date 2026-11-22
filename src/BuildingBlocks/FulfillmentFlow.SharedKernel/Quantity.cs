namespace FulfillmentFlow.SharedKernel;

public readonly record struct Quantity : IComparable<Quantity>
{
    public static Quantity Zero => throw new NotImplementedException();

    public int Value => throw new NotImplementedException();

    public bool IsPositive => throw new NotImplementedException();

    public static Result<Quantity> Create(int value) => throw new NotImplementedException();

    public static Quantity operator +(Quantity left, Quantity right) => throw new NotImplementedException();

    public static Quantity operator -(Quantity left, Quantity right) => throw new NotImplementedException();

    public static bool operator <(Quantity left, Quantity right) => throw new NotImplementedException();

    public static bool operator >(Quantity left, Quantity right) => throw new NotImplementedException();

    public static bool operator <=(Quantity left, Quantity right) => throw new NotImplementedException();

    public static bool operator >=(Quantity left, Quantity right) => throw new NotImplementedException();

    public int CompareTo(Quantity other) => throw new NotImplementedException();
}
