using Microsoft.AspNetCore.Authorization;

namespace OpenFGATalk.Authorization;

internal class OpenFgaRequirement(string relation, Func<HttpContext, string> getObject) : IAuthorizationRequirement
{
    public string Relation { get; } = relation;
    public Func<HttpContext, string> GetObject { get; } = getObject;
}
