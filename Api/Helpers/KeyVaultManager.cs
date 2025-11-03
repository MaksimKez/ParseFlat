using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Security.KeyVault.Secrets;

namespace Api.Helpers;

//this is made only to learn az key vault, of course it's not a good practice to use it in this way
public class KeyVaultManager(string prefix) : KeyVaultSecretManager
{
    public override bool Load(SecretProperties secret) => secret.Name.StartsWith(prefix);

    public override string GetKey(KeyVaultSecret secret) =>
        secret.Name[prefix.Length..].Replace("--", ConfigurationPath.KeyDelimiter);
}