using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SteamAppsDiscordBot.Services.DTO;

public class AppNews
{
    [JsonProperty("appid")]
    public int? AppId { get; set; }

    [JsonProperty("newsitems")]
    public List<NewsItem>? NewsItems { get; set; }
    
    public class NewsItem
    {
        [JsonProperty("gid")]
        public string? Gid { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("url")]
        public string? Url { get; set; }

        [JsonProperty("contents")]
        public string? Contents { get; set; }

        [JsonProperty("date")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime? Date { get; set; }

        [JsonProperty("appid")]
        public int? AppId { get; set; }
    }

    private class UnixTimestampConverter : DateTimeConverterBase
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
        }
        
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (!CanConvert(objectType)) throw new JsonSerializationException($"Unexpected type {objectType} for converting Unix timestamp.");
            if (reader.TokenType == JsonToken.Null) return null;

            if (reader.TokenType == JsonToken.Integer)
            {
                long unixTimeStamp;
                switch (reader.Value)
                {
                    case long value:
                        unixTimeStamp = value;
                        break;
                    case int intValue:
                        unixTimeStamp = intValue;
                        break;
                    case string strValue when long.TryParse(strValue, out unixTimeStamp):
                        return DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp).DateTime;
                    default:
                        throw new JsonSerializationException($"Unexpected value type {reader.Value?.GetType()} for converting Unix timestamp.");
                }

                return DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp).DateTime;
            }
            
            return DateTime.MinValue;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}