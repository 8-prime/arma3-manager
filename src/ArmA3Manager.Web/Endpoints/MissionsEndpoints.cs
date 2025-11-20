namespace ArmA3Manager.Web.Endpoints;

public static class MissionsEndpoints
{
    public static WebApplication MapMissionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("missions");
        return app;
    }
}