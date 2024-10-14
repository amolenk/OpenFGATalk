using Microsoft.AspNetCore.Authorization;
using OpenFga.Sdk.Client;
using OpenFga.Sdk.Client.Model;

namespace OpenFGATalk.Authorization;

internal class OpenFgaAuthorizationHandler(IHttpContextAccessor httpContextAccessor, OpenFgaClient openFgaClient) 
    : AuthorizationHandler<OpenFgaRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OpenFgaRequirement requirement)
    {
        var request = new ClientCheckRequest {
            User = $"user:{context.User.Identity!.Name}",
            Relation = requirement.Relation,
            Object = requirement.GetObject(httpContextAccessor.HttpContext!)
        };
        
        var response = await openFgaClient.Check(request);
        
        if (response.Allowed == true)
        {
            context.Succeed(requirement);
        }
    }
}