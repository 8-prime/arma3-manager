using ArmA3Manager.Web.Models;

namespace ArmA3Manager.Web.Extensions;

public static class InitializationEndpointConventionBuilderExtensions
{
    public static TBuilder RequireInitialization<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.Add(endpointBuilder => { endpointBuilder.Metadata.Add(new RequireInitializationMetadata()); });

        return builder;
    }
}