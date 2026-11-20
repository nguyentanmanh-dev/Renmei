using Renmei.Application;
using Renmei.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    if (app.Environment.IsDevelopment())
    {
        var db = scope.ServiceProvider.GetRequiredService<Renmei.Infrastructure.Persistence.ApplicationDbContext>();
        await db.Database.MigrateAsync();
    }

    await scope.ServiceProvider.GetRequiredService<Renmei.Application.Abstractions.IDataSeeder>().SeedAsync();
}

app.UseExceptionHandler();
if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.MapHealthChecks("/health");
app.MapControllers();
app.Run();

public partial class Program { 
}