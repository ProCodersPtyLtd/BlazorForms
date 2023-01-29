param location string = resourceGroup().location
param envName string = 'Development'
param appName string
param containerImage string
param containerPort int
param registry string
param registryUsername string

@secure()
param registryPassword string

var environmentConfigurationMap = {
  Production: loadJsonContent('config-Production.json')
}

var envConfig = environmentConfigurationMap[envName]
var keyVaultName = '${envConfig.environmentName}-platz-kv'
var keyVaultUrl = 'https://${keyVaultName}${environment().suffixes.keyvaultDns}'

module law 'bicep/law.bicep' = {
    name: 'log-analytics'
    params: {
      location: location
      name: 'log-analytics'
    }
}

module containerAppEnvironment 'bicep/app-environment.bicep' = {
  name: 'app-environment'
  params: {
    name: 'app-environment'
    location: location
    lawClientId:law.outputs.clientId
    lawClientSecret: law.outputs.clientSecret
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
}

resource dataProtectionKey 'Microsoft.KeyVault/vaults/keys@2022-07-01' = {
  name: '${appName}-data-protection-key'
  parent: keyVault
  properties: {
    kty: 'RSA'
    keySize: 2048
  }
}

module blobStore 'bicep/storage.bicep' = {
  name: '${appName}-store'
  params: {
    location: location
    svcStorageAccountName: guid(envConfig.environmentName, 'app-environment')
    svcStorageDataProtectionContainerName: appName
    tags: {
    }
  }
}

module containerApp 'bicep/app.bicep' = {
  name: appName
  dependsOn: [
    keyVault
  ]
  params: {
    name: appName
    location: location
    containerAppEnvironmentName: containerAppEnvironment.name
    domainName: envConfig[appName].domainName
    certificateName: envConfig[appName].certificateName
    certificateValue: keyVault.getSecret(envConfig[appName].certificateName)
    containerImage: containerImage
    containerPort: containerPort
    envVars: [
      {
        name: 'ASPNETCORE_ENVIRONMENT'
        value: 'Production'
      }
      {
        name: 'DataProtectionBlobUri'
        value: blobStore.outputs.dataProtectionBlobUri
      }
      {
        name: 'DataProtectionKeyUri'
        value: dataProtectionKey.properties.keyUri
      }
      {
        name: 'KeyVaultUrl'
        value: keyVaultUrl
      }
    ]
    useExternalIngress: true
    registry: registry
    registryUsername: registryUsername
    registryPassword: registryPassword
    cpu: envConfig.cpu
    memory: envConfig.memory
  }
}

module storageAccess 'bicep/storage-access.bicep' = {
  name: '${appName}-storage-access'
  params: {
    conrtibutorPrincipalId: containerApp.outputs.appPrincipalId
    svcStorageAccountName: blobStore.outputs.storageAccountName
  }
}

resource keyVaultPolicy 'Microsoft.KeyVault/vaults/accessPolicies@2022-07-01' = {
  name: '${envConfig.environmentName}-platz-kv/add'
  properties: {
    accessPolicies: [
      {
        objectId: containerApp.outputs.appPrincipalId
        permissions: {
          secrets: [
            'get', 'list', 'purge', 'delete'
          ]
          keys: [
            'Encrypt', 'Decrypt', 'Sign', 'Get', 'List', 'UnwrapKey'
          ]
        }
        tenantId: subscription().tenantId
      }
    ]
  }
}


output fqdn string = containerApp.outputs.fqdn
