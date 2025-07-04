using System.Text.Json.Serialization;

namespace BlazorTool.Client.Models
{
    public class IdentityData
    {
        [JsonPropertyName("personID")]
        public int PersonID { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("token")]
        public string Token { get; set; } = default!;

        [JsonPropertyName("langCode")]
        public string LangCode { get; set; } = default!;

        [JsonPropertyName("canHaveManyActiveTake")]
        public bool CanHaveManyActiveTake { get; set; }

        [JsonPropertyName("RigthMatrix")]
        public RightMatrix RigthMatrix { get; set; } = default!;

        [JsonPropertyName("expires")]
        public DateTime Expires { get; set; }
    }
}
