using System.Text.Json.Serialization;

namespace WebApp.Data
{
    public class ErrorResponse
    {
        [JsonPropertyName("title")] //auta ta jsonpropertyNames einai apo auta pou dinei errors (postman)
        public string? Title { get; set; }
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("errors")]
        public Dictionary<string,List<string>>? Errors { get; set; }
    }
}
