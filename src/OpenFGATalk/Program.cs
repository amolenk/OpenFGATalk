using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

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

var app = builder.Build();

app.MapGet("/check-in", () => "You are checked in!")
    .RequireAuthorization(configure => configure.RequireRole("passenger"));

app.MapGet("/access-fast-lane", () => "You are allowed in the security fast lane!")
    .RequireAuthorization(configure =>  configure.RequireAssertion(context => 
    {
        var isCheckedIn = context.User.HasClaim(claim => claim is { Type: "checked_in", Value: "true" });
        var isPremiumFrequentFlyer = context.User.HasClaim(claim => claim is { Type: "frequent_flyer_status", Value: "silver" or "gold" or "platinum" });
            
        return isCheckedIn && isPremiumFrequentFlyer;
    }));

app.MapGet("/access-lounge", () => Results.Forbid());

app.Run();
