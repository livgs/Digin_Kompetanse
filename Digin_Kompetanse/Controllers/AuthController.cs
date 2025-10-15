using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Digin_Kompetanse.data;
using Digin_Kompetanse.Services;
using Digin_Kompetanse.Models;
using Microsoft.Extensions.Logging;

namespace Digin_Kompetanse.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly KompetanseContext _db;
    private readonly IOtpRateLimiter _limiter;
    private readonly IOtpService _otp;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        KompetanseContext db,
        IOtpRateLimiter limiter,
        IOtpService otp,
        ILogger<AuthController> logger)
    {
        _db = db;
        _limiter = limiter;
        _otp = otp;
        _logger = logger;
    }

    // 📩 Send engangskode (OTP)
    [HttpPost("request-otp")]
    public async Task<IActionResult> RequestOtp([FromBody] RequestOtpDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest(new { message = "E-post mangler." });

        if (!_limiter.CanRequest(dto.Email))
            return StatusCode(429, new { message = "For mange forespørsler, prøv senere." });

        var bedrift = await _db.Bedrift.FirstOrDefaultAsync(b => b.BedriftEpost == dto.Email);
        if (bedrift == null)
            return NotFound(new { message = "E-post er ikke registrert." });

        var code = await _otp.GenerateOtpAsync(dto.Email);

        _logger.LogInformation("OTP for {Email}: {Code}", dto.Email, code);

        return Ok(new { message = "Kode sendt (dev: sjekk serverlogg)." });
    }

    // ✅ Verifiser OTP og logg inn
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Code))
            return BadRequest(new { message = "Kode mangler." });

        var isValid = await _otp.VerifyOtpAsync(dto.Email, dto.Code);
        if (!isValid)
            return BadRequest(new { message = "Ugyldig eller utløpt kode." });

        var bedrift = await _db.Bedrift.FirstOrDefaultAsync(b => b.BedriftEpost == dto.Email);
        if (bedrift == null)
            return NotFound(new { message = "Bedrift ikke funnet." });

        // 🔹 Fjern eventuell admin-session først
        HttpContext.Session.Remove("AdminId");
        HttpContext.Session.Remove("Role");

        // 🔹 Lagre innlogget bedrift i session
        HttpContext.Session.SetInt32("BedriftId", bedrift.BedriftId);
        HttpContext.Session.SetString("Role", "Bedrift");

        _logger.LogInformation("Bedrift {Navn} (ID: {Id}) logget inn.", bedrift.BedriftNavn, bedrift.BedriftId);

        return Ok(new { message = "Innlogging vellykket!", bedriftId = bedrift.BedriftId });
    }

    // 🚪 Logg ut bedrift (Alternativ B)
    [HttpPost("logout")]
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
