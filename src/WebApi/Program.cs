using Application;
using Hellang.Middleware.ProblemDetails;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using WebApi;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;

//register serilog
builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApiKeyAuthentication(builder.Configuration)
    .AddSwagger()
    .AddValidation(assembly)
    .AddProblemDetailsHandler();

builder.Services.AddControllers();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseProblemDetails();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();