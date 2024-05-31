using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
	public static IEnumerable<IdentityResource> IdentityResources =>
			[
						new IdentityResources.OpenId(),
						new IdentityResources.Profile(),
			];

	public static IEnumerable<ApiScope> ApiScopes =>
			[
						new ApiScope("auctionApp", "Auction App API"),
			];

	public static IEnumerable<Client> Clients =>
			[
					new Client{
						ClientId = "postman",
						ClientName = "Postman Client",
						AllowedScopes = { "openid", "profile", "auctionApp"},
						RedirectUris = { "https://www.getpostman.com/oauth2/callback" },
						ClientSecrets = [
							new Secret("NotASecret".Sha256())
						],
						AllowedGrantTypes = { GrantType.ResourceOwnerPassword },
					},
					// for web app
					new Client
					{
						ClientId = "nextApp",
						ClientName = "nextApp",
						ClientSecrets = {
							new Secret("NotASecret".Sha256())
						},
						AllowedGrantTypes = GrantTypes.CodeAndClientCredentials ,
						RequirePkce = false,
						RedirectUris = { "http://localhost:3000/api/auth/callback/duende-identity-server6" },
						AllowOfflineAccess = true,
						AllowedScopes = { "openid", "profile", "auctionApp"},
						// 30 days
						AccessTokenLifetime = 3600 * 24 * 30,
						AlwaysIncludeUserClaimsInIdToken = true,
					},
					new Client
					{
						ClientId = "nextAppProd",
						ClientName = "nextAppProd",
						ClientSecrets = {
							new Secret("NotASecretProd".Sha256())
						},
						AllowedGrantTypes = GrantTypes.CodeAndClientCredentials ,
						RequirePkce = false,
						RedirectUris = { "https://mandiri-ui.farhandev.cloud/api/auth/callback/duende-identity-server6" },
						AllowOfflineAccess = true,
						AllowedScopes = { "openid", "profile", "auctionApp"},
						// 30 days
						AccessTokenLifetime = 3600 * 24 * 30,
						AlwaysIncludeUserClaimsInIdToken = true,
					},
			];
}
