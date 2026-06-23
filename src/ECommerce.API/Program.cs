using ECommerce.API;
using ECommerce.Infrastructure;
using ECommerce.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentation();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApplication();

var app = builder.Build();


app.Run();