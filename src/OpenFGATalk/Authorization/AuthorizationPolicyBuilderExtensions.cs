using Microsoft.AspNetCore.Authorization;

namespace OpenFGATalk.Authorization;

public static class AuthorizationPolicyBuilderExtensions
{
    public static AuthorizationPolicyBuilder RequireOpenFgaCheck(
        this AuthorizationPolicyBuilder builder, 
        string relation,
        Func<HttpContext, string> getObject)
    {
        builder.Requirements.Add(new OpenFgaRequirement(relation, getObject));
        return builder;
    }
}
