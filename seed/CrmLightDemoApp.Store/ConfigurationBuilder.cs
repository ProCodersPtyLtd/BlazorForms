using System.Diagnostics;
using Azure.Identity;

namespace CrmLightDemoApp;

public static class ConfigurationBuilder
{
    public static void ConfigureAzureKeyVaultConfiguration(this ConfigurationManager configurationManager)
    {
        var keyVaultUrl = configurationManager.GetSection("KeyVaultUrl").Value;

        if (!string.IsNullOrWhiteSpace(keyVaultUrl))
        {
            var defaultAzureCredentialOptions = new DefaultAzureCredentialOptions
            {
                ExcludeEnvironmentCredential = true,
                ExcludeSharedTokenCacheCredential = true,
                ExcludeManagedIdentityCredential = Debugger.IsAttached
            };

            configurationManager.AddAzureKeyVault(new Uri(keyVaultUrl),
                new DefaultAzureCredential(defaultAzureCredentialOptions));
        }
    }
}
