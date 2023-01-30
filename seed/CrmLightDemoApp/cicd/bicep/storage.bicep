param location string
param tags object


///////////////////////////////////
// Resource names
param svcStorageAccountName string
param svcStorageDataProtectionContainerName string

param svcStorageAccountNameDashless string = replace(svcStorageAccountName, '-', '')
param svcStorageAccountNameShort string = substring(svcStorageAccountNameDashless, 0, length(svcStorageAccountNameDashless) > 24 ? 24 : length(svcStorageAccountNameDashless))

///////////////////////////////////
// New resources

resource svcStorageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' = {
  name: svcStorageAccountNameShort
  location: location
  tags: tags
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    accessTier: 'Hot'
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    allowBlobPublicAccess: true
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
  }
  resource blobServices 'blobServices' = {
    name: 'default'
    properties: {
      deleteRetentionPolicy: {
        enabled: true
        allowPermanentDelete: true
        days: 7
      }
    }
  }
}

@description('A blob container that will be used to store the ASP.NET Core Data Protection keys')
resource dataProtectionContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-09-01' = {
  name: '${svcStorageAccount.name}/default/${svcStorageDataProtectionContainerName}'
  properties: {
    publicAccess: 'None'
  }
}

output storageAccountName string = svcStorageAccount.name
output storageBlobPrimaryEndpoint string = svcStorageAccount.properties.primaryEndpoints.blob
output dataProtectionBlobUri string = '${svcStorageAccount.properties.primaryEndpoints.blob}${svcStorageDataProtectionContainerName}/keys.xml'
