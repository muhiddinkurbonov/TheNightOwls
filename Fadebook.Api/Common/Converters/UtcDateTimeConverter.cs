using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fadebook.Common.Converters;

/// <summary>
/// JSON converter that ensures all DateTime values are treated as UTC
/// </summary>
public class UtcDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateTimeString = reader.GetString();
        if (string.IsNullOrEmpty(dateTimeString))
        {
            return DateTime.MinValue;
        }

        // Parse the datetime and ensure it's treated as UTC
        if (DateTime.TryParse(dateTimeString, out var dateTime))
        {
            // If the datetime doesn't have a timezone specified, treat it as UTC
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            }

            // Convert to UTC if it's a local time
            if (dateTime.Kind == DateTimeKind.Local)
            {
                return dateTime.ToUniversalTime();
            }

            return dateTime;
        }

        throw new JsonException($"Unable to parse datetime: {dateTimeString}");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // Always write datetime as UTC in ISO 8601 format
        var utcValue = value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : value;

        // Ensure the value is marked as UTC
        if (utcValue.Kind == DateTimeKind.Unspecified)
        {
            utcValue = DateTime.SpecifyKind(utcValue, DateTimeKind.Utc);
        }

        writer.WriteStringValue(utcValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
    }
}
