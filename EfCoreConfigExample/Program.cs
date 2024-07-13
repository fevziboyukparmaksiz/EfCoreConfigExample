using EfCoreConfigExample;
using EfCoreConfigExample.Entities;
using EfCoreConfigExample.Models;
using EfCoreConfigExample.Options;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureOptions<DatabaseOptionsSetup>();

builder.Services.AddDbContext<AppDbContext>(
    (serviceProvider,options) =>
{
    var databaseOptions = serviceProvider.GetService<IOptions<DatabaseOptions>>()!.Value;

    options.UseSqlServer(databaseOptions.ConnectionString, sqlServerOptionsAction =>
    {
        sqlServerOptionsAction.EnableRetryOnFailure(databaseOptions.MaxRetryCount);
        sqlServerOptionsAction.CommandTimeout(databaseOptions.CommandTimeOut);
    });

    options.EnableDetailedErrors(databaseOptions.EnableDetailedErrors);

    options.EnableSensitiveDataLogging(databaseOptions.EnableSensitiveDataLogging);

});


var app = builder.Build();

app.UseHttpsRedirection();


app.MapGet("companies/{companyId:int", async (int companyId, AppDbContext context) =>
{
    var company = await context
        .Set<Company>()
        .AsNoTracking()
        .FirstOrDefaultAsync(c => c.Id == companyId);

    if (company is null)
    {
        return Results.NotFound($"The company with Id '{companyId}' was not found.");
    }

    var response = new CompanyResponse(company.Id, company.Name);

    return Results.Ok(response);
});


app.Run();
