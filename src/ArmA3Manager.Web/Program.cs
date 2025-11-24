using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Application.Common.Models;
using ArmA3Manager.Application.Services;
using ArmA3Manager.Web.Endpoints;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ManagerSettings>(builder.Configuration);
builder.Services.AddHostedService<InitializationManager>();
builder.Services.AddSingleton<IServerManager, ServerManager>();
builder.Services.AddSingleton<IInitializeable>(sp => sp.GetRequiredService<IServerManager>());
builder.Services.AddSingleton<IMissionsManager, MissionManager>();
builder.Services.AddSingleton<IInitializeable>(sp => sp.GetRequiredService<IMissionsManager>());
builder.Services.AddSingleton<IConfigManager, ConfigManager>();
builder.Services.AddSingleton<IInitializeable>(sp => sp.GetRequiredService<IConfigManager>());
builder.Services.AddSingleton<IUpdatesQueue<string>, UpdatesQueue<string>>();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
}

app
    .MapConfigEndpoints()
    .MapServerManagementEndpoints()
    .MapMissionEndpoints()
    .MapModEndpoints();
app.MapGet("/healthz", () => "up");

app.Run();