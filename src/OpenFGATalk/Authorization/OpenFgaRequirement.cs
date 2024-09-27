using Microsoft.AspNetCore.Authorization;

namespace OpenFGATalk.Authorization;

internal class OpenFgaRequirement : IAuthorizationRequirement
{
    public OpenFgaRequirement(string relation, string obj)
        : this(null, relation, obj)
    {
    }

    public OpenFgaRequirement(string? user, string relation, string obj)
    {
        User = user;
        Relation = relation;
        Object = obj;
    }
    
    public string? User { get; }
    public string Relation { get; }
    public string Object { get; }
}
