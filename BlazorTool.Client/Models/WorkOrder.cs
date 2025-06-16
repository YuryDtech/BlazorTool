using System;
using System.Text.Json.Serialization;

namespace BlazorTool.Client.Models
{
    public class WorkOrder
    {
        [JsonPropertyName("workOrderID")]
        public int WorkOrderID { get; set; }

        [JsonPropertyName("machineID")]
        public int MachineID { get; set; }

        [JsonPropertyName("asset_No")]
        public string? AssetNo { get; set; }

        [JsonPropertyName("device_Category")]
        public string? DeviceCategory { get; set; }

        [JsonPropertyName("wO_Desc")]
        public string? WODesc { get; set; }

        [JsonPropertyName("wO_Category")]
        public string? WOCategory { get; set; }

        [JsonPropertyName("wO_State")]
        public string? WOState { get; set; }

        [JsonPropertyName("wO_Reason")]
        public string? WOReason { get; set; }

        [JsonPropertyName("add_Date")]
        public DateTime? AddDate { get; set; }

        [JsonPropertyName("take_Date")]
        public DateTime? TakeDate { get; set; }

        [JsonPropertyName("close_Date")]
        public DateTime? CloseDate { get; set; }

        [JsonPropertyName("cost")]
        public decimal? Cost { get; set; }

        [JsonPropertyName("take_Persons")]
        public string? TakePersons { get; set; }

        [JsonPropertyName("planID")]
        public int? PlanID { get; set; }

        [JsonPropertyName("state_Color")]
        public string? StateColor { get; set; }

        [JsonPropertyName("mod_Date")]
        public DateTime? ModDate { get; set; }

        [JsonPropertyName("mod_Person")]
        public string? ModPerson { get; set; }

        [JsonPropertyName("wO_Level")]
        public string? WOLevel { get; set; }

        [JsonPropertyName("levelID")]
        public int? LevelID { get; set; }

        [JsonPropertyName("act_Count")]
        public int? ActCount { get; set; }

        [JsonPropertyName("dep_Name")]
        public string DepName { get; set; } = string.Empty;

        [JsonPropertyName("assigned_Person")]
        public string AssignedPerson { get; set; } = string.Empty;

        [JsonPropertyName("file_Count")]
        public int? FileCount { get; set; }

        [JsonPropertyName("part_Count")]
        public int? PartCount { get; set; }

        [JsonPropertyName("plan_Act_Count")]
        public int? PlanActCount { get; set; }

        [JsonPropertyName("start_Date")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("end_Date")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("is_Scheduled_Planned")]
        public bool IsScheduledPlanned { get; set; }

        [JsonPropertyName("categoryID")]
        public int? CategoryID { get; set; }

        [JsonPropertyName("reasonID")]
        public int? ReasonID { get; set; }

        [JsonPropertyName("ineffective_Count")]
        public int? IneffectiveCount { get; set; }

        [JsonPropertyName("person_Take_Date")]
        public DateTime? PersonTakeDate { get; set; }

        [JsonPropertyName("deviceCategoryID")]
        public int? DeviceCategoryID { get; set; }
    }
}
