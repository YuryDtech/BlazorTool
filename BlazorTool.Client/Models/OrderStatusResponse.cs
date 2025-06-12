using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlazorTool.Client.Models
{
    public class OrderStatusResponse
    {
        [JsonPropertyName("data")]
        public List<OrderStatus> Data { get; set; } = new();

        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("errors")]
        public List<string> Errors { get; set; } = new();
    }

    public class OrderStatus
    {
        [JsonPropertyName("machineCategoryID")]
        public int? MachineCategoryID { get; set; }

        [JsonPropertyName("is_Default")]
        public bool IsDefault { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("listType")]
        public int ListType { get; set; }
    }
}
