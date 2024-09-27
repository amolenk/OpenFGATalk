using Microsoft.AspNetCore.Authorization;
using OpenFga.Sdk.Client;

namespace OpenFGATalk.Authorization;

public static class Extensions
{
    public static WebApplicationBuilder AddOpenFgaAuthorization(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration.GetSection("OpenFga").Get<ClientConfiguration>();
        if (configuration is null)
        {
            throw new InvalidOperationException("OpenFGA configuration is missing.");
        }
        
        builder.Services
            .AddSingleton<OpenFgaClient>(_ => new OpenFgaClient(configuration))
            .AddSingleton<IAuthorizationHandler, OpenFgaAuthorizationHandler>();

        return builder;
    }

    public static AuthorizationPolicyBuilder RequireOpenFgaCheck(this AuthorizationPolicyBuilder builder, string relation, string obj)
    {
        builder.Requirements.Add(new OpenFgaRequirement(relation, obj));
        return builder;
    }

    public static AuthorizationPolicyBuilder RequireOpenFgaCheck(this AuthorizationPolicyBuilder builder, string user, string relation, string obj)
    {
        builder.Requirements.Add(new OpenFgaRequirement(user, relation, obj));
        return builder;
    }
}