namespace IdentityService.Configuration;

public class NextAppClientConfig : ClientConfig
{
	public NextAppClientConfig(string clientId, string clientSecrets, string redirectUris) :
			base(clientId, clientSecrets, redirectUris)
	{ }
}