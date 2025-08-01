using Newtonsoft.Json;

namespace BlazorTool.Client.Models
{
    public class AddActivity
    {
            [JsonProperty("workOrderID")]
            public int WorkOrderID { get; set; }

            [JsonProperty("personID")]
            public int PersonID { get; set; }

            [JsonProperty("categoryID")]
            public int CategoryID { get; set; }

            [JsonProperty("work_Load")]
            public decimal WorkLoad { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

    }
}
