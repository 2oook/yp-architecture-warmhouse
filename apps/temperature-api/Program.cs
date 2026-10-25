using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// GET /temperature?location=Living Room
// GET /temperature?sensorId=1
app.MapGet("/temperature", (string? location, string? sensorId) =>
    Results.Ok(TemperatureGenerator.Generate(location, sensorId)));

// GET /temperature/1 - используется монолитом smart_home для получения температуры по ID датчика
app.MapGet("/temperature/{sensorId}", (string sensorId) =>
    Results.Ok(TemperatureGenerator.Generate(null, sensorId)));

app.Run();

static class TemperatureGenerator
{
    private const double MinTemperature = 18.0;
    private const double MaxTemperature = 28.0;

    public static TemperatureResponse Generate(string? location, string? sensorId)
    {
        location ??= string.Empty;
        sensorId ??= string.Empty;

        // If no location is provided, use a default based on sensor ID
        if (location == string.Empty)
        {
            location = sensorId switch
            {
                "1" => "Living Room",
                "2" => "Bedroom",
                "3" => "Kitchen",
                _ => "Unknown"
            };
        }

        // If no sensor ID is provided, generate one based on location
        if (sensorId == string.Empty)
        {
            sensorId = location switch
            {
                "Living Room" => "1",
                "Bedroom" => "2",
                "Kitchen" => "3",
                _ => "0"
            };
        }

        var value = Math.Round(MinTemperature + Random.Shared.NextDouble() * (MaxTemperature - MinTemperature), 1);

        return new TemperatureResponse(
            Value: value,
            Unit: "°C",
            Timestamp: DateTime.UtcNow,
            Location: location,
            Status: "active",
            SensorId: sensorId,
            SensorType: "temperature",
            Description: $"Temperature sensor in {location}");
    }
}

record TemperatureResponse(
    [property: JsonPropertyName("value")] double Value,
    [property: JsonPropertyName("unit")] string Unit,
    [property: JsonPropertyName("timestamp")] DateTime Timestamp,
    [property: JsonPropertyName("location")] string Location,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("sensor_id")] string SensorId,
    [property: JsonPropertyName("sensor_type")] string SensorType,
    [property: JsonPropertyName("description")] string Description);
