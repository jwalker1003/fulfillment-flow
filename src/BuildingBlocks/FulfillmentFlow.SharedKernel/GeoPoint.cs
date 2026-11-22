namespace FulfillmentFlow.SharedKernel;

public readonly record struct GeoPoint
{
    public double Latitude => throw new NotImplementedException();

    public double Longitude => throw new NotImplementedException();

    public static Result<GeoPoint> Create(double latitude, double longitude) => throw new NotImplementedException();

    public double DistanceKmTo(GeoPoint other) => throw new NotImplementedException();
}
