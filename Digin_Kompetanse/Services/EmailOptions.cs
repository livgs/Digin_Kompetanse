namespace Digin_Kompetanse.Services
{
    public class EmailOptions
    {
        public string Host { get; set; } = "";
        public int Port { get; set; } = 587;
        public string User { get; set; } = "";
        public string Pass { get; set; } = "";
        public string From { get; set; } = "";
        public bool EnableStartTls { get; set; } = true;
    }
}