using Digin_Kompetanse.data;
using Digin_Kompetanse.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

builder.Services.AddDbContext<KompetanseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<KompetanseContext>();
    db.Database.Migrate();
}

app.Run();
