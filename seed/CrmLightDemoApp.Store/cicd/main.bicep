param location string = resourceGroup().location
param envName string = 'Development'
param appName string
param containerImage string
param containerPort int
param registry string
param registryUsername string
@secure()
param registryPassword string


module config 'bicep/my-environment.bicep' = {
  name: '${appName}-environment-config'
  params: {
    environment: envName
  }
}

module app 'bicep/app-with-store.bicep' = {
  name: '${appName}-app-with-store'
  params: {
    location: location
    appName: appName
    config: config.outputs.config
    containerImage: containerImage
    containerPort: containerPort
    registry: registry
    registryPassword: registryPassword
    registryUsername: registryUsername
  }
}
