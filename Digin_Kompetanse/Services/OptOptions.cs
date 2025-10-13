namespace Digin_Kompetanse.Services;

public class OtpOptions
{
    public int CodeLength { get; set; } = 6;
    public TimeSpan Ttl { get; set; } = TimeSpan.FromMinutes(5);
    public int MaxRequestsPerEmailPerHour { get; set; } = 5;
}