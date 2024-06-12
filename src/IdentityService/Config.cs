using Duende.IdentityServer.Models;
using IdentityService.Configuration;
using Microsoft.Extensions.Options;

namespace IdentityService;

public static class Config
{
	// IConfiguration

	public static IEnumerable<IdentityResource> IdentityResources =>
			[
						new IdentityResources.OpenId(),
						new IdentityResources.Profile(),
			];

	public static IEnumerable<ApiScope> ApiScopes =>
			[
						new ApiScope("auctionApp", "Auction App API"),
			];

	public static IEnumerable<Client> Get(WebApplicationBuilder builder)
	{

		PostManClientConfig postManClientConfig = builder.Configuration.GetSection("Client:PostMan").Get<PostManClientConfig>();
		NextAppClientConfig nextAppClientConfig = builder.Configuration.GetSection("Client:NextApp").Get<NextAppClientConfig>();

		return [
					new Client{
						ClientId = postManClientConfig.ClientId,
						ClientName = "postman",
						AllowedScopes = { "openid", "profile", "auctionApp"},
						RedirectUris = { postManClientConfig.RedirectUris },
						ClientSecrets = [
							new Secret(postManClientConfig.ClientSecrets.Sha256())
						],
						AllowedGrantTypes = { GrantType.ResourceOwnerPassword },
					},
					// for web app
					new Client
					{
						ClientId = nextAppClientConfig.ClientId,
						ClientName = "nextApp",
						ClientSecrets = {
							new Secret(nextAppClientConfig.ClientSecrets.Sha256())
						},
						AllowedGrantTypes = GrantTypes.CodeAndClientCredentials ,
						RequirePkce = false,
						RedirectUris = { nextAppClientConfig.RedirectUris },
						AllowOfflineAccess = true,
						AllowedScopes = { "openid", "profile", "auctionApp"},
						// 30 days
						AccessTokenLifetime = 3600 * 24 * 30,
						AlwaysIncludeUserClaimsInIdToken = true,
					},
			];
	}
}
