using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Digin_Kompetanse.data;
using Digin_Kompetanse.Services;
using Digin_Kompetanse.Models;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Digin_Kompetanse.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AuthController : Controller
{
    private readonly KompetanseContext _db;
    private readonly IOtpRateLimiter _limiter;
    private readonly IOtpService _otp;
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _config;

    public AuthController(
        KompetanseContext db,
        IOtpRateLimiter limiter,
        IOtpService otp,
        ILogger<AuthController> logger,
        IConfiguration config)
    {
        _db = db;
        _limiter = limiter;
        _otp = otp;
        _logger = logger;
        _config = config;
    }

    [HttpPost("request-otp")]
    public async Task<IActionResult> RequestOtp([FromBody] RequestOtpDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest(new { message = "E-post mangler." });

        if (!_limiter.CanRequest(dto.Email))
            return StatusCode(429, new { message = "For mange forespørsler, prøv senere." });

        var email = dto.Email.Trim().ToLowerInvariant();
        var bedrift = await _db.Bedrift.FirstOrDefaultAsync(b => b.BedriftEpost.ToLower() == email);
        if (bedrift == null)
            return NotFound(new { message = "E-post er ikke registrert." });

        var code = await _otp.GenerateOtpAsync(email);
        if (code is null)
            return StatusCode(429, new { message = "Kunne ikke sende kode nå. Prøv igjen om litt." });

        try
        {
            var smtpHost = _config["SMTP_HOST"] ?? "";
            var smtpPort = int.TryParse(_config["SMTP_PORT"], out var p) ? p : 587;
            var smtpUser = _config["SMTP_USER"] ?? "";
            var smtpPass = _config["SMTP_PASS"] ?? "";
            var smtpFrom = _config["SMTP_FROM"] ?? smtpUser;
            var enableStartTls = (_config["SMTP_ENABLE_STARTTLS"] ?? "true").ToLowerInvariant() == "true";

#if DEBUG
            _logger.LogInformation("SMTP cfg (debug): host={Host}, port={Port}, user={UserMasked}, starttls={Tls}",
                smtpHost, smtpPort, MaskEmail(smtpUser), enableStartTls);
#endif

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(smtpFrom));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Din engangskode (OTP)";
            message.Body = new BodyBuilder
            {
                HtmlBody = $@"
                    <p>Hei {bedrift.BedriftNavn},</p>
                    <p>Din engangskode er: <strong style='font-size:20px'>{code}</strong></p>
                    <p>Koden er gyldig i 5 minutter.</p>
                    <p>Hvis du ikke forsøkte å logge inn, kan du se bort fra denne e-posten.</p>",
                TextBody = $"Din engangskode er: {code} (gyldig i 5 minutter)."
            }.ToMessageBody();

            using var client = new SmtpClient();

            SecureSocketOptions security =
                smtpPort == 465 ? SecureSocketOptions.SslOnConnect :
                enableStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;

            await client.ConnectAsync(smtpHost, smtpPort, security);

            if (!string.IsNullOrWhiteSpace(smtpUser))
                await client.AuthenticateAsync(smtpUser, smtpPass);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("OTP e-post sendt til {EmailMasked}.", MaskEmail(email));
        }
        catch (SmtpCommandException ex)
        {
            _logger.LogError(ex, "SMTP command error ({Status}) til {EmailMasked}", ex.StatusCode, MaskEmail(email));
            return StatusCode(500, new { message = "Kunne ikke sende e-post (SMTP-feil)." });
        }
        catch (SslHandshakeException ex)
        {
            _logger.LogError(ex, "SSL/TLS-feil mot SMTP for {EmailMasked}", MaskEmail(email));
            return StatusCode(500, new { message = "Kunne ikke sende e-post (TLS-feil)." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved utsending av OTP e-post til {EmailMasked}", MaskEmail(email));
            return StatusCode(500, new { message = "Kunne ikke sende e-post. Prøv igjen senere." });
        }

        return Ok(new { message = "Kode sendt. Sjekk e-posten din." });
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Code))
            return BadRequest(new { message = "Kode mangler." });

        var isValid = await _otp.VerifyOtpAsync(dto.Email, dto.Code);
        if (!isValid)
            return BadRequest(new { message = "Ugyldig eller utløpt kode." });

        var bedrift = await _db.Bedrift.FirstOrDefaultAsync(b => b.BedriftEpost.ToLower() == dto.Email.Trim().ToLowerInvariant());
        if (bedrift == null)
            return NotFound(new { message = "Bedrift ikke funnet." });

        HttpContext.Session.Remove("AdminId");
        HttpContext.Session.Remove("Role");

        HttpContext.Session.SetInt32("BedriftId", bedrift.BedriftId);
        HttpContext.Session.SetString("Role", "Bedrift");

        _logger.LogInformation("Bedrift (ID: {Id}) logget inn.", bedrift.BedriftId);

        return Ok(new { message = "Innlogging vellykket!", bedriftId = bedrift.BedriftId });
    }

    [HttpPost("logout-bedrift")]
    public IActionResult LogoutBedrift()
    {
        var role = HttpContext.Session.GetString("Role");
        var bedriftId = HttpContext.Session.GetInt32("BedriftId");

        if (role != "Bedrift" || bedriftId == null)
        {
            TempData["Message"] = "Ingen bedrift er logget inn.";
            TempData["MessageType"] = "warning"; // kan brukes for farge på alert
            return RedirectToAction("Index", "Home"); // send til ønsket side
        }

        HttpContext.Session.Remove("BedriftId");
        HttpContext.Session.Remove("Role");

        _logger.LogInformation("Bedrift (ID: {BedriftId}) logget ut.", bedriftId);
        TempData["Message"] = "Du er nå logget ut.";
        TempData["MessageType"] = "success";
        return RedirectToAction("Index", "Home");
    }

    // — helpers (maskering) —
    private static string MaskEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return "";
        var parts = email.Split('@');
        if (parts.Length != 2) return "***";
        var local = parts[0];
        var domain = parts[1];
        var visible = local.Length <= 2 ? local[..1] : local[..2];
        return $"{visible}***@{domain}";
    }
}

public class RequestOtpDto
{
    public string Email { get; set; } = null!;
}
public class VerifyOtpDto
{
    public string Email { get; set; } = null!;
    public string Code { get; set; } = null!;
}
