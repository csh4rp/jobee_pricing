using System.Reflection;
using JasperFx;
using JasperFx.Events.Projections;
using Jobee.Pricing.Api;
using Jobee.Pricing.Api.Endpoints;
using Jobee.Pricing.Infrastructure;
using Jobee.Pricing.Infrastructure.DataAccess;
using Marten;
using Wolverine;
using Wolverine.Marten;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration)
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

builder.UseWolverine(o =>
{
    o.Discovery.IncludeAssembly(Assembly.Load("Jobee.Pricing.Application"));
    o.Discovery.IncludeAssembly(Assembly.Load("Jobee.Pricing.Infrastructure"));
});

builder.Host.ApplyJasperFxExtensions();
var app =  builder.Build();

app.AddProductEndpoints()
    .UseSwagger()
    .UseSwaggerUI();

await app.RunAsync();