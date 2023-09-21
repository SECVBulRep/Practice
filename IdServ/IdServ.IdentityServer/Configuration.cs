using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdServ.IdentityServer;

public static class Configuration
{
    public static IEnumerable<Client> GetClients() => new List<Client>
    {
        new Client
        {
            ClientId = "client_id",
            ClientSecrets = { new Secret("client_secret".ToSha256()) },
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            AllowedScopes = { "OrdersAPI" },
        },
        new Client
        {
            ClientId = "client_id_mvc",
            ClientSecrets = { new Secret("client_secret_mvc".ToSha256()) },
            AllowedGrantTypes = GrantTypes.Code,
            AllowedScopes =
            {
                "OrdersAPI",
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
            },
            RedirectUris = { "https://localhost:5048/signin-oidc" },
            //AlwaysIncludeUserClaimsInIdToken = true
            AccessTokenLifetime = 5
        }
    };


    public static IEnumerable<ApiResource> GetApiResources() => new List<ApiResource>
    {
        new ApiResource
        {
            Name = "OrdersAPI",
            Scopes = { "OrdersAPI" }
        }
    };


    public static IEnumerable<IdentityResource> GetIdentityResources() => new List<IdentityResource>
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
    };

    public static IEnumerable<ApiScope> GetApiScopes() => new List<ApiScope>
    {
        new ApiScope
        {
            Name = "OrdersAPI"
        }
    };
}