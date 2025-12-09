using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Application.Common.Models;
using ArmA3Manager.Application.Common.Models.Server;
using ArmA3Manager.Application.Services;
using ArmA3Manager.Web.Endpoints;
using ArmA3Manager.Web.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Allow large file uploads for mods
builder.WebHost.ConfigureKestrel(options => { options.Limits.MaxRequestBodySize = int.MaxValue; });
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue;
});

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
builder.Services.AddSingleton<IInitializationInfo, InitializationInfo>();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
// DOTNET would _like_ you to use MapStaticAssets. Cool, I love me some good gzip.
// Please fix https://github.com/dotnet/aspnetcore/issues/59399 then.
app.UseStaticFiles();
app.UseDefaultFiles();
app.UseInitialization();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
}

app
    .MapInitializationEndpoints()
    .MapConfigEndpoints()
    .MapServerManagementEndpoints()
    .MapMissionEndpoints()
    .MapModEndpoints();
app.MapGet("/healthz", () => "up");

await app.RunAsync();