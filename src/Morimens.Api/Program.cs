using Morimens.Application.Calculator;
using Morimens.Core.Interfaces;
using Morimens.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ── Services ─────────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// Domain / Application
builder.Services.AddSingleton<IAwakenerRepository, InMemoryAwakenerRepository>();
builder.Services.AddSingleton<IDamageCalculator,   DamageCalculator>();

// CORS — allow the React dev server and WPF localhost origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// ── Middleware ────────────────────────────────────────────────────────────────
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
