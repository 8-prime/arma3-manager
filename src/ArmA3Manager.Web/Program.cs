using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Application.Common.Models;
using ArmA3Manager.Application.Common.Models.Server;
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
builder.Services.AddSingleton<IModsManager, ModsManager>();
builder.Services.AddSingleton<IInitializeable>(sp => sp.GetRequiredService<IModsManager>());
builder.Services.AddSingleton<IUpdatesQueue<string>, UpdatesQueue<string>>();
builder.Services.AddSingleton<IUpdatesQueue<ServerLogEntry>, UpdatesQueue<ServerLogEntry>>();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app.MapStaticAssets();
app.UseDefaultFiles();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
}

app
    .MapConfigEndpoints()
    .MapServerManagementEndpoints()
    .MapMissionEndpoints()
    .MapModEndpoints()
    .MapModEndpoints();
app.MapGet("/healthz", () => "up");

await app.RunAsync();