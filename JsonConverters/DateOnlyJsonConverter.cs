using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace finansee_api.JsonConverters;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string DateFormat = "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected string token for DateOnly value.");

        var s = reader.GetString() ?? string.Empty;

        // Try parse pure date
        if (DateOnly.TryParseExact(s, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
            return d;

        // Fallback: try parse as DateTime and take date part (accepts ISO datetimes)
        if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dt))
            return DateOnly.FromDateTime(dt);

        // Last resort: try general DateOnly parse with invariant culture
        if (DateOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
            return d;

        throw new JsonException($"Unable to parse DateOnly from '{s}'. Expected format {DateFormat} or ISO datetime.");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}
