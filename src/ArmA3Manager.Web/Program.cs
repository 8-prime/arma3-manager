using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Application.Common.Models;
using ArmA3Manager.Application.Services;
using ArmA3Manager.Web.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ManagerSettings>(builder.Configuration);
builder.Services.AddSingleton<IServerManager, ServerManager>();
builder.Services.AddHostedService<IServerManager>(sp => sp.GetRequiredService<IServerManager>());
builder.Services.AddSingleton<IMissionsManager, MissionManager>();
builder.Services.AddSingleton<IUpdatesQueue<string>, UpdatesQueue<string>>();

var app = builder.Build();

app
    .MapConfigEndpoints()
    .MapManagementEndpoints()
    .MapMissionEndpoints()
    .MapModEndpoints();
app.MapGet("/healthz", () => "up");

app.Run();