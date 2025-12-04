using AspNetCoreRateLimit;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using StaffAttApi.StartupConfig;

var builder = WebApplication.CreateBuilder(args);

builder.AddStandardServices();
builder.AddCustomServices();
builder.AddIdentityServices();
builder.AddAuthServices();
builder.AddHealthCheckServices();
builder.AddRateLimitServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseIpRateLimiting();
app.UseResponseCaching();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    }).AllowAnonymous();
    app.MapHealthChecksUI(options => options.UIPath = "/health-ui").AllowAnonymous();
}
else
{
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    }).RequireAuthorization();
    app.MapHealthChecksUI(options => options.UIPath = "/health-ui").RequireAuthorization();
}

app.MapControllers();
await IdentitySeedExtensions.SeedRolesAndAdminAsync(app.Services);
app.Run();
