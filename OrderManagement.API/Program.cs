using OrderManagement.API.Extensions;
using OrderManagement.API.Middlewares;
using Scalar.AspNetCore;
using Serilog;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Infrastructure.Data;

// Configura o Serilog antes de tudo
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando OrderManagement API...");

    var builder = WebApplication.CreateBuilder(args);

    // ─── Serilog ─────────────────────────────────────────────
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

    // ─── Services ────────────────────────────────────────────
    builder.Services.AddControllers();
    builder.Services.AddValidators();
    builder.Services.AddDatabase(builder.Configuration);
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddRepositories();
    builder.Services.AddServices();
    builder.Services.AddScalarDocumentation();
    builder.Services.AddCorsPolicy();       
    builder.Services.AddRateLimiting();      
    builder.Services.AddHealthChecks(builder.Configuration);

    // ─── Build ───────────────────────────────────────────────
    var app = builder.Build();

    // ─── Middleware Pipeline ──────────────────────────────────
    app.UseMiddleware<ExceptionMiddleware>();
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} → {StatusCode} ({Elapsed:0.000}ms)";
    });

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.Title = "Order Management API";
            options.Theme = ScalarTheme.DeepSpace;
        });
    }

    // app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseCors("AllowAll");
    app.UseRateLimiter();
    app.MapHealthChecks("/health");
    app.MapControllers();

    // Aplica migrations automaticamente ao subir
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        Log.Information("Migrations aplicadas com sucesso.");
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "API encerrada inesperadamente.");
}
finally
{
    Log.CloseAndFlush();
}