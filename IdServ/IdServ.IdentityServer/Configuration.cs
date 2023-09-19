using IdentityModel;
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
        }
    };


    public static IEnumerable<ApiResource> GetApiResources() => new List<ApiResource>
    {
        {
            new ApiResource
            {
                Name = "OrdersAPI",
                Scopes = { "OrdersAPI" }
            }
        }
    };


    public static IEnumerable<IdentityResource> GetIdentityResources() => new List<IdentityResource>
    {
        { new IdentityResources.OpenId() }
    };

    public static IEnumerable<ApiScope> GetApiScopes() => new List<ApiScope>
    {
        new ApiScope
        {
            Name = "OrdersAPI"
        }
    };
}