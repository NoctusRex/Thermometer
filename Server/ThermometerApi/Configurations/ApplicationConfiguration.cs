namespace Configurations
{
    public class ApplicationConfiguration
    {
        public string JwtKey { get; set; } = "";
        public string JwtIssuer { get; set; } = "";
        public string JwtAudience { get; set; } = "";

        public int Port { get; set; } = 80;
        public bool UseHttps { get; set; } = false;
        public bool UseCors { get; set; } = false;

        public string DatabaseServer { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string DatabasePassword { get; set; } = string.Empty;
        public string DatabaseUser { get; set; } = string.Empty;
        public string DatabasePort { get; set; } = string.Empty;
    }
}
