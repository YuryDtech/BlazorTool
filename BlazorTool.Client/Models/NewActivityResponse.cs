using Newtonsoft.Json;
namespace BlazorTool.Client.Models
{
    public class NewActivityResponse
    {
    //"activityID": 220,
    //"workOrderID": 217,
    //"description": "Test",
    //"categoryID": 1,
    //"work_Load": 1,
    //"cost": 0.00,
    //"workers": 1
    [JsonProperty("activityID")]
    public int ActivityID { get; set; }

    [JsonProperty("workOrderID")]
    public int WorkOrderID { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("categoryID")]
    public int CategoryID { get; set; }

    [JsonProperty("work_Load")]
    public int WorkLoad { get; set; }

    [JsonProperty("cost")]
    public decimal Cost { get; set; }

    [JsonProperty("workers")]
    public int Workers { get; set; }

    }

    public class AddToActivityResponse
    {
        [JsonProperty("activityID")]
        public int ActivityID { get; set; }
       
        [JsonProperty("work_Load")]
        public decimal WorkLoad { get; set; }

        [JsonProperty("personID")]
        public int PersonID { get; set; }

        [JsonProperty("activityPersonID")]
        public int ActivityPersonID { get; set; }
        
        [JsonProperty("add_Date")]
        public DateTime AddDate { get; set; }
    }
}
