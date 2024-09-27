using Microsoft.AspNetCore.Authorization;
using OpenFga.Sdk.Client;
using OpenFga.Sdk.Client.Model;

namespace OpenFGATalk.Authorization;

// This class contains logic for determining whether OpenFgaRequirement in authorization
// policies are satisfied or not.
internal class OpenFgaAuthorizationHandler : AuthorizationHandler<OpenFgaRequirement>
{
    private readonly OpenFgaClient _openFgaClient;

    public OpenFgaAuthorizationHandler(OpenFgaClient openFgaClient)
    {
        ArgumentNullException.ThrowIfNull(openFgaClient);
        
        _openFgaClient = openFgaClient;
    }

    // Check whether a given MinimumAgeRequirement is satisfied or not for a particular context
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OpenFgaRequirement requirement)
    {
        var user = requirement.User ?? $"user:{context.User.Identity!.Name}";
        
        var request = new ClientCheckRequest {
            User = user,
            Relation = requirement.Relation,
            Object = requirement.Object
        };
    
        var response = await _openFgaClient.Check(request);
        
        if (response.Allowed == true)
        {
            context.Succeed(requirement);
        }
    }
}