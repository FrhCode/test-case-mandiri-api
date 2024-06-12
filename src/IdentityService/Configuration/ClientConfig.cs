namespace IdentityService.Configuration;

public class ClientConfig
{
	public string ClientId { get; set; }
	public string ClientSecrets { get; set; }
	public string RedirectUris { get; set; }

	public ClientConfig(string clientId, string clientSecrets, string redirectUris)
	{
		ClientId = clientId;
		ClientSecrets = clientSecrets;
		RedirectUris = redirectUris;
	}
}