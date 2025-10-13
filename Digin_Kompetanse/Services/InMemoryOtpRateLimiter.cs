namespace Digin_Kompetanse.Services;

public interface IOtpRateLimiter
{
    bool CanRequest(string email);
    void RegisterRequest(string email);
}

public class InMemoryOtpRateLimiter : IOtpRateLimiter
{
    private readonly Dictionary<string, List<DateTime>> _requests = new();
    private readonly object _lock = new();

    public bool CanRequest(string email)
    {
        lock (_lock)
        {
            var now = DateTime.UtcNow;
            if (!_requests.ContainsKey(email))
                _requests[email] = new List<DateTime>();

            // Fjern gamle forespørsler (>1 time gamle)
            _requests[email] = _requests[email]
                .Where(t => (now - t).TotalHours < 1)
                .ToList();

            // Tillat maks 5 per time
            return _requests[email].Count < 5;
        }
    }

    public void RegisterRequest(string email)
    {
        lock (_lock)
        {
            if (!_requests.ContainsKey(email))
                _requests[email] = new List<DateTime>();
            _requests[email].Add(DateTime.UtcNow);
        }
    }
}