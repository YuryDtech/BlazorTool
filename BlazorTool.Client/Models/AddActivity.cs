using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BlazorTool.Client.Models
{
    public class AddActivity
    {
            [JsonProperty("workOrderID")]
            public int WorkOrderID { get; set; }

            [Required(ErrorMessage = "Person is required")]
            [Range(1, int.MaxValue, ErrorMessage = "Person must be selected.")]
            [JsonProperty("personID")]
            public int PersonID { get; set; }

            [Required(ErrorMessage = "Category is required")]
            [Range(1,int.MaxValue, ErrorMessage = "Category must be selected.")]
            [JsonProperty("categoryID")]
            public int CategoryID { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Workload must be a positive number.")]
            [JsonProperty("workLoad")]
            public decimal WorkLoad { get; set; } = 0;

            [Required(ErrorMessage = "Description is required")]
            [JsonProperty("description")]
            public string Description { get; set; }

    }

    public class JoinToActivity
    {
        [Required(ErrorMessage = "Person is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Person must be selected.")]
        [JsonProperty("personID")]
        public int PersonID { get; set; }

        [Required(ErrorMessage = "Activity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Activity must be selected.")]
        [JsonProperty("activityID")]
        public int ActivityID { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Workload must be a positive number.")]
        [JsonProperty("work_Load")]
        public decimal WorkLoad { get; set; } = 0;
    }
}
