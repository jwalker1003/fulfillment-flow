using FulfillmentFlow.SharedKernel;

namespace FulfillmentFlow.SharedKernel.Tests;

public sealed class GeoPointTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(90, 180)]
    [InlineData(-90, -180)]
    [InlineData(47.6062, -122.3321)]
    [Trait("Req", "NFR-7")]
    public void NFR_7_point_within_range_is_created(double latitude, double longitude)
    {
        var result = GeoPoint.Create(latitude, longitude);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Latitude.ShouldBe(latitude);
        result.Value.Longitude.ShouldBe(longitude);
    }

    [Theory]
    [InlineData(90.0001, 0)]
    [InlineData(-90.0001, 0)]
    [InlineData(0, 180.0001)]
    [InlineData(0, -180.0001)]
    [InlineData(double.NaN, 0)]
    [InlineData(0, double.NaN)]
    [InlineData(double.PositiveInfinity, 0)]
    [Trait("Req", "NFR-7")]
    public void NFR_7_point_out_of_range_is_rejected(double latitude, double longitude)
    {
        var result = GeoPoint.Create(latitude, longitude);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe(SharedErrors.InvalidGeoPoint);
        result.Error.Code.ShouldBe("shared.invalid-geo-point");
    }

    [Fact]
    [Trait("Req", "NFR-7")]
    public void NFR_7_distance_seattle_to_chicago_is_about_2795_km()
    {
        var seattle = GeoPoint.Create(47.6062, -122.3321).Value;
        var chicago = GeoPoint.Create(41.8781, -87.6298).Value;

        var distance = seattle.DistanceKmTo(chicago);

        distance.ShouldBe(2795, tolerance: 10);
        chicago.DistanceKmTo(seattle).ShouldBe(distance, tolerance: 0.000001);
    }

    [Fact]
    [Trait("Req", "NFR-7")]
    public void NFR_7_distance_to_the_same_point_is_zero()
    {
        var point = GeoPoint.Create(47.6062, -122.3321).Value;

        var distance = point.DistanceKmTo(point);

        distance.ShouldBe(0, tolerance: 0.000001);
    }

    [Fact]
    [Trait("Req", "NFR-7")]
    public void NFR_7_points_with_the_same_coordinates_are_equal()
    {
        var first = GeoPoint.Create(1.5, 2.5).Value;
        var second = GeoPoint.Create(1.5, 2.5).Value;

        var equal = first == second;

        equal.ShouldBeTrue();
        (first == GeoPoint.Create(1.5, 2.6).Value).ShouldBeFalse();
    }
}
