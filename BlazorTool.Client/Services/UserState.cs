namespace BlazorTool.Client.Services
{
    public class UserState
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Token { get; set; }
        public void Clear()
        {
            UserName = null;
            Password = null;
            Token = null;
        }
    }
}
