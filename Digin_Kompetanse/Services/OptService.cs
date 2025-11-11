using System.Security.Cryptography;
using System.Text;
using Digin_Kompetanse.data;
using Digin_Kompetanse.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Digin_Kompetanse.Services;

public interface IOtpService
{
    Task<string?> GenerateOtpAsync(string email);
    Task<bool> VerifyOtpAsync(string email, string code);
}

public class OtpService : IOtpService
{
    private readonly KompetanseContext _context;
    private readonly IOtpRateLimiter _rateLimiter;
    private readonly OtpOptions _options;

    public OtpService(
        KompetanseContext context,
        IOtpRateLimiter rateLimiter,
        IOptions<OtpOptions> options)
    {
        _context = context;
        _rateLimiter = rateLimiter;
        _options = options.Value;
    }

    public async Task<string?> GenerateOtpAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        // Normaliser e-post
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var bedrift = await _context.Bedrift
            .FirstOrDefaultAsync(b => b.BedriftEpost.ToLower() == normalizedEmail);

        if (bedrift == null)
            return null;

        // Rate limiting på normalisert e-post
        if (!_rateLimiter.CanRequest(normalizedEmail))
            return null;

        // Generer kode
        var code = RandomNumberGenerator
            .GetInt32(0, (int)Math.Pow(10, _options.CodeLength))
            .ToString("D" + _options.CodeLength);

        // Lagre hash i databasen
        var token = new LoginToken
        {
            BedriftId = bedrift.BedriftId,
            CodeHash = Hash(code),
            ExpiresAt = DateTime.UtcNow.Add(_options.Ttl),
            Attempts = 0
        };

        _context.LoginToken.Add(token);
        await _context.SaveChangesAsync();

        _rateLimiter.RegisterRequest(normalizedEmail);

        return code;
    }

    public async Task<bool> VerifyOtpAsync(string email, string code)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
            return false;

        var normalizedEmail = email.Trim().ToLowerInvariant();

        var bedrift = await _context.Bedrift
            .FirstOrDefaultAsync(b => b.BedriftEpost.ToLower() == normalizedEmail);

        if (bedrift == null)
            return false;

        var token = await _context.LoginToken
            .Where(t => t.BedriftId == bedrift.BedriftId && t.ConsumedAt == null)
            .OrderByDescending(t => t.Id)
            .FirstOrDefaultAsync();

        if (token == null)
            return false;

        if (token.ExpiresAt < DateTime.UtcNow)
            return false;

        if (token.CodeHash != Hash(code))
        {
            token.Attempts++;
            await _context.SaveChangesAsync();
            return false;
        }

        token.ConsumedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    private static string Hash(string input)
    {
        using var sha = SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
    }
}
