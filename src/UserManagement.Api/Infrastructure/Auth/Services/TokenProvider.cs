using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;

namespace UserManagement.Api.Infrastructure.Auth.Services;

public class TokenProvider(IConfiguration config) : IAccessTokenProvider
{
    public AllowedHostsValidator? AllowedHostsValidator { get; }

    public async Task<string> GetAuthorizationTokenAsync(Uri uri,
                                                   Dictionary<string, object>? additionalAuthenticationContext = null,
                                                   CancellationToken cancellationToken = default)
    {
        var clientId = config.GetSection("EntraId").GetValue<string>("ClientId");
        var secret = config.GetValue<string>("EntraIdClientSecret");
        var authority = config.GetSection("EntraId").GetValue<string>("Authority");

        var app = ConfidentialClientApplicationBuilder.Create(clientId)
            .WithAuthority(authority)
            .WithClientSecret(secret)
            .Build();

        var result = await app.AcquireTokenForClient(["https://graph.microsoft.com/.default"]).ExecuteAsync();

        return result.AccessToken;
    }
}
