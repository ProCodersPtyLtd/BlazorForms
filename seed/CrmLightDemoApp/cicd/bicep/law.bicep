param location string
param name string

resource law 'Microsoft.OperationalInsights/workspaces@2021-12-01-preview' = {
  name: name
  location: location
  properties: any({
    retentionInDays: 30
    features: {
      searchVersion: 1
    }
    sku: {
      name: 'PerGB2018'
    }
  })
}
output clientId string = law.properties.customerId
output clientSecret string = law.listKeys().primarySharedKey
