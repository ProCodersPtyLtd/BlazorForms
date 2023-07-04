using System.Diagnostics;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.DataProtection;

namespace CrmLightDemoApp;

public static class AzureExtensions
{
    public static DefaultAzureCredential DefaultAzureCredential(this IConfiguration configuration)
    {
        var azureTenantId = configuration.GetSection("AZURE_TENANT_ID").Value;
        var kvCredential = new DefaultAzureCredential(
            new DefaultAzureCredentialOptions
            {
                TenantId = azureTenantId,
                ExcludeEnvironmentCredential = true,
                ExcludeSharedTokenCacheCredential = true,
                ExcludeManagedIdentityCredential = Debugger.IsAttached
            });
        return kvCredential;
    }
    
    public static IServiceCollection AddAzureDataProtection(this IServiceCollection services, IConfiguration configuration)
    {
        var dataProtection = services.AddDataProtection();

        if (bool.TryParse(configuration["DataProtection:UseLocalStore"], out var useLocalStore) == false ||
            useLocalStore == false)
        {
            var azureCredential = configuration.DefaultAzureCredential();

            var blobUri = new Uri(configuration["DataProtectionBlobUri"] ??
                                  throw new InvalidOperationException(
                                      $"Config value 'DataProtectionBlobUri' not set"));

            // use StoreName and StoreKey for local debug only
            var dataProtectionStoreName = configuration["DataProtection:StoreName"];
            var dataProtectionStoreKey = configuration["DataProtection:StoreKey"];

            if (string.IsNullOrEmpty(dataProtectionStoreName) || string.IsNullOrEmpty(dataProtectionStoreKey))
            {
                dataProtection.PersistKeysToAzureBlobStorage(blobUri, azureCredential);
            }
            else
            {
                var storageCredential =
                    new StorageSharedKeyCredential(dataProtectionStoreName, dataProtectionStoreKey);
                var blobClient = new BlobClient(blobUri, storageCredential);
                dataProtection.PersistKeysToAzureBlobStorage(blobClient);
            }

            dataProtection.ProtectKeysWithAzureKeyVault(
                new Uri(configuration[$"DataProtectionKeyUri"] ??
                        throw new InvalidOperationException($"Config value 'DataProtectionKeyUri' not set")),
                azureCredential);
        }

        return services;
    }
}