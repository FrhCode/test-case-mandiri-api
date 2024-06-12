namespace IdentityService.Configuration;

public class PostManClientConfig : ClientConfig
{
    public PostManClientConfig(string clientId, string clientSecrets,string redirectUris) : 
        base(clientId, clientSecrets, redirectUris) { }
}