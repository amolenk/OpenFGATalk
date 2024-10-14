using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OpenFga.Sdk.Client;
using OpenFga.Sdk.Client.Model;
using OpenFGATalk.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add Authentication and Authorization
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = false,
            SignatureValidator = (token, _) => new JsonWebToken(token)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyClient", policy =>
    {
        policy.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAnyClient");

app.MapGet("/check-in/{flightNumber}", (string flightNumber, HttpContext context) =>
    {
        var claims = context.User.Claims;
        
        return "You are checked in,\nhave a good flight!";
    })
    .RequireAuthorization(policy => policy.RequireRole("ticket_holder"));

app.MapGet("/priority-lane", () =>
    {
        return "You are allowed in the security priority lane!";
    })
    .RequireAuthorization(policy => policy
        .RequireRole("passenger")
        .RequireAssertion(context => 
        {
            var frequentFlyerStatus = context.User.FindFirst(claim => claim.Type == "frequent_flyer_status")?.Value;

            return frequentFlyerStatus is "jetsetter" or "unobtanium";
        }));

app.MapGet("/lounge", () => "You are allowed in the lounge!")
    .RequireAuthorization(policy => policy.RequireAssertion(_ =>
        {
            return false;
        }));

app.MapGet("/lounges", Array.Empty<string>);

#region

// app.MapGet("/check-in/{flightNumber}", async (string flightNumber, HttpContext context, OpenFgaClient fgaClient) =>
//     {
//         var user = context.User.Identity!.Name;
//         
//         var request = new ClientWriteRequest() {
//             Writes =
//             [
//                 new ClientTupleKey
//                 {
//                     User = $"user:{user}",
//                     Relation = "passenger",
//                     Object = $"flight:{flightNumber}"
//                 }
//             ],
//             Deletes = 
//             [
//                 new ClientTupleKeyWithoutCondition
//                 {
//                     User = $"user:{user}",
//                     Relation = "ticket_holder",
//                     Object = $"flight:{flightNumber}"
//                 }
//             ]
//         };
//         
//         await fgaClient.Write(request);
//         
//         return "You are checked in!";
//     })
//     .RequireAuthorization(policy => policy
//         .RequireOpenFgaCheck("can_check_in", context => $"flight:{context.Request.RouteValues["flightNumber"]}"));
//
// app.MapGet("/priority-lane", () =>
//     {
//         return "You are allowed in the security priority lane!";
//     })
//     .RequireAuthorization(policy => policy.RequireOpenFgaCheck("is_eligible", _ => "frequent_flyer_benefit:priority_lane"));
//
// app.MapGet("/lounge", () =>
//     {
//         return "You are allowed in the lounge!";
//     })
//     .RequireAuthorization(policy => policy.RequireOpenFgaCheck("has_access", _ => "lounge:azure"));
//
// app.MapGet("/lounges", async (HttpContext context, OpenFgaClient fgaClient) =>
//     {
//         var user = context.User.Identity!.Name;
//         
//         var request = new ClientListObjectsRequest {
//             User = $"user:{user}",
//             Relation = "has_access",
//             Type = "lounge"
//         };
//         
//         var response = await fgaClient.ListObjects(request);
//
//         switch (response.Objects.Count)
//         {
//             case 0:
//                 return "You don't have access to any lounges.";
//             case 1:
//                 return $"You have access to the '{response.Objects[0]}' lounge!";
//             default:
//                 return $"You have access to the {string.Join(" and ", response.Objects)} lounges!";
//         }
//     });

#endregion

app.Run();
