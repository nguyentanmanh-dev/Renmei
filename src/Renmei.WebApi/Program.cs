using Renmei.Application;
using Renmei.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Renmei.Application.Abstractions;
using Renmei.WebApi.Services;
using Renmei.Application.Features.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddSingleton(TimeProvider.System);

var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
byte[] signingKeyBytes = [];
if (!string.IsNullOrWhiteSpace(jwt.SigningKey))
{
try { signingKeyBytes = Convert.FromBase64String(jwt.SigningKey); }
catch (FormatException) { /* ValidateOnStart() trong AddInfrastructure sẽ báo lỗi rõ khi khởi động */ }
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
{
o.MapInboundClaims = false;
o.TokenValidationParameters = new TokenValidationParameters
{
ValidateIssuer = true,
ValidIssuer = jwt.Issuer,
ValidateAudience = true,
ValidAudience = jwt.Audience,
ValidateIssuerSigningKey = true,
IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),
ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
ValidateLifetime = true,
ClockSkew = TimeSpan.FromSeconds(30),
NameClaimType = "sub",
RoleClaimType = "role"
};
});

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
if (app.Environment.IsDevelopment())
{
var db = scope.ServiceProvider.GetRequiredService<Renmei.Infrastructure.Persistence.ApplicationDbContext>();
await db.Database.MigrateAsync();
}

await scope.ServiceProvider.GetRequiredService<IDataSeeder>().SeedAsync();
}

app.UseExceptionHandler();
if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();
app.Run();

public partial class Program
{
}