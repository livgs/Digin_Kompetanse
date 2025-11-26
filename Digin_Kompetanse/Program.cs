using Digin_Kompetanse.data;
using Digin_Kompetanse.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Npgsql; 
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<KompetanseContext>(options =>
{
    var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
    var dbName = Environment.GetEnvironmentVariable("DB_NAME");
    var dbUser = Environment.GetEnvironmentVariable("DB_USER");
    var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
    var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";

    string connectionString;

    if (!string.IsNullOrWhiteSpace(dbHost) &&
        !string.IsNullOrWhiteSpace(dbName) &&
        !string.IsNullOrWhiteSpace(dbUser))
    {
        var csb = new NpgsqlConnectionStringBuilder
        {
            Host = dbHost,
            Port = int.Parse(dbPort),
            Database = dbName,
            Username = dbUser,
            Password = dbPassword ?? string.Empty,
        };

        connectionString = csb.ToString();

        Console.WriteLine($"[DB] Using env connection: Host={csb.Host}, Port={csb.Port}, Db={csb.Database}, User={csb.Username}");
    }
    else
    {
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                           ?? throw new InvalidOperationException("DefaultConnection mangler i konfigurasjon.");
        Console.WriteLine("[DB] Using DefaultConnection fra config (ingen DB_HOST satt).");
    }

    options.UseNpgsql(connectionString);
});

builder.Services.AddControllersWithViews(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/keys"))
    .SetApplicationName("DiginKompetanse");

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddHttpContextAccessor();

// Kun OTP-konfig
builder.Services.Configure<OtpOptions>(o =>
{
    o.CodeLength = 6;
    o.Ttl = TimeSpan.FromMinutes(5);
    o.MaxRequestsPerEmailPerHour = 5;
});

// Tjenester
builder.Services.AddSingleton<IOtpRateLimiter, InMemoryOtpRateLimiter>();
builder.Services.AddScoped<IOtpService, OtpService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseMiddleware<SimpleIpRateLimitMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// En enkel health-check for Docker / load balancer
app.MapGet("/live", () => Results.Ok("OK"));

// En veldig enkel /metrics – kan gjøres mer avansert senere
app.MapGet("/metrics", () =>
{
    // Her kan du senere koble på ekte metrikker
    var text = "digin_requests_total 1\n";
    return Results.Text(text, "text/plain");
});

app.Run();
