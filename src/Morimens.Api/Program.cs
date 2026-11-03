using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Morimens.Api.Services;
using Morimens.Application.Calculator;
using Morimens.Core.Interfaces;
using Morimens.Infrastructure.Data;
using Morimens.Infrastructure.Identity;
using Morimens.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<MorimensDbContext>(opts =>
    opts.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=morimens.db"));

// ── Identity ──────────────────────────────────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opts =>
{
    opts.Password.RequireDigit           = true;
    opts.Password.RequiredLength         = 8;
    opts.Password.RequireNonAlphanumeric = false;
    opts.User.RequireUniqueEmail         = true;
})
.AddEntityFrameworkStores<MorimensDbContext>()
.AddDefaultTokenProviders();

// ── JWT ───────────────────────────────────────────────────────────────────────
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("Jwt:Secret must be set in configuration.");

builder.Services
    .AddAuthentication(opts =>
    {
        opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opts.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer           = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"]   ?? "morimens-api",
            ValidateAudience         = true,
            ValidAudience            = builder.Configuration["Jwt:Audience"] ?? "morimens-clients",
            ClockSkew                = TimeSpan.FromSeconds(30),
        };
    })
    .AddDiscord(opts =>
    {
        opts.ClientId     = builder.Configuration["Discord:ClientId"]
            ?? throw new InvalidOperationException("Discord:ClientId must be set.");
        opts.ClientSecret = builder.Configuration["Discord:ClientSecret"]
            ?? throw new InvalidOperationException("Discord:ClientSecret must be set.");
        opts.Scope.Add("email");
        opts.SaveTokens = true;
    });

// ── Application services ──────────────────────────────────────────────────────
builder.Services.AddSingleton<IAwakenerRepository,      InMemoryAwakenerRepository>();
builder.Services.AddSingleton<IDamageCalculator,        DamageCalculator>();
builder.Services.AddScoped<IUserProfileRepository,      UserProfileRepository>();
builder.Services.AddScoped<ISavedSimulationRepository,  SavedSimulationRepository>();
builder.Services.AddScoped<IRefreshTokenRepository,     RefreshTokenRepository>();
builder.Services.AddScoped<JwtService>();

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", p =>
        p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddControllers();

var app = builder.Build();

// ── Auto-migrate on startup ───────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MorimensDbContext>();
    db.Database.Migrate();
}

// ── Middleware ────────────────────────────────────────────────────────────────
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
