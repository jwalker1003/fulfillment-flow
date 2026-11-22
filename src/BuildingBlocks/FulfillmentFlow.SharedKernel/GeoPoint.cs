namespace FulfillmentFlow.SharedKernel;

/// <summary>A latitude/longitude pair in degrees.</summary>
public readonly record struct GeoPoint
{
    private const double EarthRadiusKm = 6371.0;

    private GeoPoint(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public double Latitude { get; }

    public double Longitude { get; }

    public static Result<GeoPoint> Create(double latitude, double longitude)
    {
        if (!(latitude is >= -90 and <= 90) || !(longitude is >= -180 and <= 180))
        {
            return new DomainError(
                SharedErrors.InvalidGeoPoint,
                $"Latitude must be in [-90, 90] and longitude in [-180, 180] (was {latitude}, {longitude}).");
        }

        return Result.Success(new GeoPoint(latitude, longitude));
    }

    /// <summary>Great-circle distance in kilometres (haversine, earth radius 6371 km).</summary>
    public double DistanceKmTo(GeoPoint other)
    {
        var deltaLatitude = ToRadians(other.Latitude - Latitude);
        var deltaLongitude = ToRadians(other.Longitude - Longitude);

        var a = (Math.Sin(deltaLatitude / 2) * Math.Sin(deltaLatitude / 2))
                + (Math.Cos(ToRadians(Latitude)) * Math.Cos(ToRadians(other.Latitude))
                   * Math.Sin(deltaLongitude / 2) * Math.Sin(deltaLongitude / 2));

        return EarthRadiusKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
}
