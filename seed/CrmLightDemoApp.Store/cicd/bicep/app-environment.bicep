param name string
param location string
param lawClientId string

@secure()
param lawClientSecret string

resource env 'Microsoft.App/managedEnvironments@2022-03-01' = {
  name: name
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: lawClientId
        sharedKey: lawClientSecret
      }
    }
  }
}

output id string = env.id
output caEnvDefaultDomain string = env.properties.defaultDomain
output caEnvStaticIp string = env.properties.staticIp
