using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
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

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("check-in", policy => policy.RequireRole("passenger"))
    .AddPolicy("fast-lane", policy => policy.RequireAssertion(context => 
    {
        var isCheckedIn = context.User.HasClaim(claim => claim is { Type: "checked_in", Value: "true" });
        
        var isPremiumFrequentFlyer = context.User.HasClaim(
            claim => claim is { Type: "frequent_flyer_status", Value: "silver" or "gold" or "platinum" });
            
        return isCheckedIn && isPremiumFrequentFlyer;
    }))
    .AddPolicy("lounge", policy => policy.RequireAssertion(_ => false))
    .AddPolicy("check-in", policy => policy.RequireOpenFgaCheck("can-check-in", "flight:KL1571"));



var app = builder.Build();

app.MapGet("/check-in", () => "You are checked in!")
    .RequireAuthorization("check-in");

app.MapGet("/access-fast-lane", () => "You are allowed in the security fast lane!")
    .RequireAuthorization("fast-lane");

app.MapGet("/access-lounge", () => "You are allowed in the lounge!")
    .RequireAuthorization("lounge");

app.Run();
