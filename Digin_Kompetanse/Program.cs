using Digin_Kompetanse.data;
using Digin_Kompetanse.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<KompetanseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<OtpOptions>(o =>
{
    o.CodeLength = 6;
    o.Ttl = TimeSpan.FromMinutes(5);
    o.MaxRequestsPerEmailPerHour = 5;
});

// E-postinnstillinger fra env
builder.Services.Configure<EmailOptions>(o =>
{
    o.Host = builder.Configuration["SMTP_HOST"] ?? "";
    o.Port = int.TryParse(builder.Configuration["SMTP_PORT"], out var p) ? p : 587;
    o.User = builder.Configuration["SMTP_USER"] ?? "";
    o.Pass = builder.Configuration["SMTP_PASS"] ?? "";
    o.From = builder.Configuration["SMTP_FROM"] ?? o.User;
    o.EnableStartTls = (builder.Configuration["SMTP_ENABLE_STARTTLS"] ?? "true").ToLowerInvariant() == "true";
});

// Bytt til MailKit
builder.Services.AddSingleton<IEmailSender, MailKitEmailSender>();

builder.Services.AddSingleton<IOtpRateLimiter, InMemoryOtpRateLimiter>();
builder.Services.AddScoped<IOtpService, OtpService>();

builder.Services.AddControllersWithViews(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();