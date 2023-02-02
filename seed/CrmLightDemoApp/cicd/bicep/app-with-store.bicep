param location string = resourceGroup().location
param appName string
param config object
param containerImage string
param containerPort int
param registry string
param registryUsername string
@secure()
param registryPassword string

module law 'law.bicep' = {
    name: 'log-analytics'
    params: {
      location: location
      name: 'log-analytics'
    }
}

module containerAppEnvironment 'app-environment.bicep' = {
  name: 'app-environment'
  params: {
    name: 'app-environment'
    location: location
    lawClientId: law.outputs.clientId
    lawClientSecret: law.outputs.clientSecret
  }
}

var keyVaultName = '${config.environmentName}-platz-kv'
var keyVaultUrl = 'https://${keyVaultName}${environment().suffixes.keyvaultDns}'

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

module blobStore 'storage.bicep' = {
  name: '${appName}-store'
  params: {
    location: location
    svcStorageAccountName: guid(config.environmentName, 'app-environment')
    svcStorageDataProtectionContainerName: appName
    tags: {
    }
  }
}

module containerApp 'app.bicep' = {
  name: '${appName}-app'
  dependsOn: [
    keyVault
  ]
  params: {
    name: appName
    location: location
    containerAppEnvironmentName: containerAppEnvironment.name
    domainName: config[appName].domainName
    certificateName: config[appName].certificateName
    certificateValue: keyVault.getSecret(config[appName].certificateName)
    containerImage: containerImage
    containerPort: containerPort
    env: {
      'ASPNETCORE_ENVIRONMENT':'Production'
      'DataProtectionBlobUri': blobStore.outputs.dataProtectionBlobUri
      'DataProtectionKeyUri': dataProtectionKey.properties.keyUri
      'KeyVaultUrl': keyVaultUrl
    }
    useExternalIngress: true
    registry: registry
    registryUsername: registryUsername
    registryPassword: registryPassword
    cpu: config.cpu
    memory: config.memory
  }
}

module storageAccess 'storage-access.bicep' = {
  name: '${appName}-storage-access'
  params: {
    conrtibutorPrincipalId: containerApp.outputs.appPrincipalId
    svcStorageAccountName: blobStore.outputs.storageAccountName
  }
}

resource keyVaultPolicy 'Microsoft.KeyVault/vaults/accessPolicies@2022-07-01' = {
  name: '${config.environmentName}-platz-kv/add'
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
