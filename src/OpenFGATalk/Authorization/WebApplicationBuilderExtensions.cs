using Microsoft.AspNetCore.Authorization;
using OpenFga.Sdk.Client;

namespace OpenFGATalk.Authorization;

public static class WebApplicationBuilderExtensions
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
}