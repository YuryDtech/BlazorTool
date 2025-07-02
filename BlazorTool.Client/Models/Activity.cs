using System.Text.Json.Serialization;

namespace BlazorTool.Client.Models
{
    public class Activity
    {
        [JsonPropertyName("workOrderID")]
        public int WorkOrderID { get; set; }

        [JsonPropertyName("activityID")]
        public int ActivityID { get; set; }

        [JsonPropertyName("work_Load")]
        public decimal WorkLoad { get; set; }

        [JsonPropertyName("workers")]
        public int Workers { get; set; }

        [JsonPropertyName("act_Category")]
        public string ActCategory { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("act_Persons")]
        public string ActPersons { get; set; }

        [JsonPropertyName("add_Date")]
        public DateTime AddDate { get; set; }
    }
}
