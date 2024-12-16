using System.Text.Json.Serialization;
using System.Text.Json;

namespace meetings_app_server.CustomConverter;


public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
{
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var timeString = reader.GetString();
        return TimeOnly.Parse(timeString); // You can handle any additional error-checking or formatting here
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("HH:mm")); // Adjust formatting as needed
    }
}
