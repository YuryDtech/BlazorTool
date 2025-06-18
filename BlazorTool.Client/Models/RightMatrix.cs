using System.Text.Json.Serialization;

namespace BlazorTool.Client.Models
{
    public class RightMatrix
    { //TODO add all properties from server RightMatrix
        [JsonPropertyName("personID")]
        public int PersonID { get; set; }

        [JsonPropertyName("wO_Add")]
        public bool WO_Add { get; set; }
        // … все остальные свойства точно так же, как в серверном RightMatrix
        [JsonPropertyName("mD_Add_ForceCycle")]
        public bool MD_Add_ForceCycle { get; set; }
    }
}
