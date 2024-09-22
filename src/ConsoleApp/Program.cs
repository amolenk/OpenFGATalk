using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

await CheckInAsync();
await AccessSecurityFastLaneAsync();
await AccessLoungeAsync();
return;


static async Task CheckInAsync()
{
    var response = await CallApiAsync("/check-in", new Claim(ClaimTypes.Role, "passenger"));
    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
    else
    {
        Console.WriteLine($"Failed ({response.StatusCode})!");
    }
}

static async Task AccessSecurityFastLaneAsync()
{
    var response = await CallApiAsync("/access-fast-lane", new Claim("checked_in", "true"), new Claim("frequent_flyer_status", "gold"));
    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
    else
    {
        Console.WriteLine($"Failed ({response.StatusCode})!");
    }
}

static async Task AccessLoungeAsync()
{
    var response = await CallApiAsync("/access-lounge", new Claim("checked_in", "true"), new Claim("frequent_flyer_status", "gold"));
    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
    else
    {
        Console.WriteLine($"Failed ({response.StatusCode})!");
    }
}


static Task<HttpResponseMessage> CallApiAsync(string requestUri, params Claim[] claims)
{
    var client = new HttpClient();
    client.BaseAddress = new Uri("https://localhost:7074/");

    var token = GenerateJwtToken(claims);
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    return client.GetAsync(requestUri);
}

static string GenerateJwtToken(params Claim[] claims)
{

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddMinutes(10)
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}