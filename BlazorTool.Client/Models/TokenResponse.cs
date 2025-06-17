namespace BlazorTool.Client.Models
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public string LangCode { get; set; }
        public DateTime Expires { get; set; } //if neasary
    }
}
