using System.Text.Json.Serialization;

namespace BlazorTool.Client.Models
{
    public class Person
    {
        [JsonPropertyName("personID")]
        public int PersonId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
