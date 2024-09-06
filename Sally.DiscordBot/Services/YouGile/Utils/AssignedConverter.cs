using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sally.DiscordBot.Services.YouGile.Utils
{
    /// <summary>
    /// Кастомный конвертер для обработки пустого поля с пользователями
    /// </summary>
    public class AssignedConverter : JsonConverter<string[]>
    {
        public override string[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType is JsonTokenType.StartArray)
            {
                var list = new List<string>();

                while (reader.Read() && reader.TokenType is not JsonTokenType.EndArray)
                {
                    list.Add(reader.GetString());
                }

                return list.ToArray();
            }
            else if (reader.TokenType is JsonTokenType.String)
            {
                return new[] { reader.GetString() };
            }

            throw new JsonException($"Unexpected token parsing assigned: {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, string[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (var item in value)
            {
                writer.WriteStringValue(item);
            }

            writer.WriteEndArray();
        }
    }
}
