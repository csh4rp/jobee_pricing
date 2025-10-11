using System.Reflection;
using JasperFx;
using Jobee.Pricing.Api.Endpoints;
using Jobee.Pricing.Infrastructure;
using Jobee.Utils.Api;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration)
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddExceptionHandlers()
    .AddValidationUtils();

builder.UseWolverine(o =>
{
    o.Discovery.IncludeAssembly(Assembly.Load("Jobee.Pricing.Application"));
    o.Discovery.IncludeAssembly(Assembly.Load("Jobee.Pricing.Infrastructure"));
});

builder.Host.ApplyJasperFxExtensions();
var app =  builder.Build();

app.AddProductEndpoints()
    .AddPackageEndpoints()
    .UseSwagger()
    .UseSwaggerUI();

await app.RunAsync();