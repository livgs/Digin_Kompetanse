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
[Route("auth")]
public class AuthController : ControllerBase
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

    // Send engangskode (OTP)
    [HttpPost("request-otp")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestOtp([FromBody] RequestOtpDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest(new { message = "E-post mangler." });

        if (!_limiter.CanRequest(dto.Email))
            return StatusCode(429, new { message = "For mange forespørsler, prøv senere." });

        var email = dto.Email.Trim().ToLowerInvariant();
        var bedrift = await _db.Bedrift.FirstOrDefaultAsync(
            b => b.BedriftEpost.ToLower() == email
        );
        if (bedrift == null)
            return NotFound(new { message = "E-post er ikke registrert." });

        // Generer OTP i tjenesten (lagres i DB som hash + utløp)
        var code = await _otp.GenerateOtpAsync(email);
        if (code is null)
        {
            // enten ratelimit, eller annet – hold feilmelding generisk
            return StatusCode(429, new { message = "Kunne ikke sende kode nå. Prøv igjen om litt." });
        }

        // 📧 Send e-post via MailKit
        try
        {
            var smtpHost = _config["SMTP_HOST"] ?? "";
            var smtpPort = int.TryParse(_config["SMTP_PORT"], out var p) ? p : 587;
            var smtpUser = _config["SMTP_USER"] ?? "";
            var smtpPass = _config["SMTP_PASS"] ?? "";
            var smtpFrom = _config["SMTP_FROM"] ?? smtpUser;
            var enableStartTls = (_config["SMTP_ENABLE_STARTTLS"] ?? "true").ToLowerInvariant() == "true";

            var message = new MimeMessage();
            // hvis SMTP_FROM er "Navn <adresse@domene.no>"
            message.From.Add(MailboxAddress.Parse(smtpFrom));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Din engangskode (OTP)";

            var html = $@"
                <p>Hei {bedrift.BedriftNavn},</p>
                <p>Din engangskode er: <strong style='font-size:20px'>{code}</strong></p>
                <p>Koden er gyldig i 5 minutter.</p>
                <p>Hvis du ikke forsøkte å logge inn, kan du se bort fra denne e-posten.</p>
            ";

            message.Body = new BodyBuilder
            {
                HtmlBody = html,
                TextBody = $"Din engangskode er: {code} (gyldig i 5 minutter)."
            }.ToMessageBody();

            using var client = new SmtpClient();

            // Velg TLS-metode basert på port/innstillinger
            SecureSocketOptions security =
                smtpPort == 465 ? SecureSocketOptions.SslOnConnect :
                enableStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;

            await client.ConnectAsync(smtpHost, smtpPort, security);

            // Auth hvis satt
            if (!string.IsNullOrWhiteSpace(smtpUser))
                await client.AuthenticateAsync(smtpUser, smtpPass);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("OTP sent to {Email}.", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved utsending av OTP e-post til {Email}", email);
            // Valgfritt: ikke røp detaljene til bruker
            return StatusCode(500, new { message = "Kunne ikke sende e-post med kode. Prøv igjen senere." });
        }

        // Dev-hjelp i loggene
        _logger.LogInformation("OTP for {Email}: {Code}", email, code); // fjern i prod!

        return Ok(new { message = "Kode sendt. Sjekk e-posten din." });
    }

    // Verifiser OTP og logg inn
    [HttpPost("verify-otp")]
    [ValidateAntiForgeryToken]
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

        // Fjern eventuell admin-session først
        HttpContext.Session.Remove("AdminId");
        HttpContext.Session.Remove("Role");

        // Lagre innlogget bedrift i session
        HttpContext.Session.SetInt32("BedriftId", bedrift.BedriftId);
        HttpContext.Session.SetString("Role", "Bedrift");

        _logger.LogInformation("Bedrift {Navn} (ID: {Id}) logget inn.", bedrift.BedriftNavn, bedrift.BedriftId);

        return Ok(new { message = "Innlogging vellykket!", bedriftId = bedrift.BedriftId });
    }
    
    [HttpPost("logout-bedrift")]
    [ValidateAntiForgeryToken]
    public IActionResult LogoutBedrift()
    {
        var role = HttpContext.Session.GetString("Role");
        var bedriftId = HttpContext.Session.GetInt32("BedriftId");

        if (role != "Bedrift" || bedriftId == null)
            return BadRequest(new { message = "Ingen bedrift er logget inn." });

        // Bare fjern Bedrift-session, la ev. admin-innlogging stå
        HttpContext.Session.Remove("BedriftId");
        HttpContext.Session.Remove("Role");

        _logger.LogInformation("Bedrift (ID: {BedriftId}) logget ut.", bedriftId);
        return Ok(new { message = "Du er nå logget ut." });
    }
}

// DTO-er
public class RequestOtpDto
{
    public string Email { get; set; } = null!;
}

public class VerifyOtpDto
{
    public string Email { get; set; } = null!;
    public string Code { get; set; } = null!;
}
