// general Azure Container App settings
param location string
param name string
param containerAppEnvironmentName string
param minReplicas int = 0
param maxReplicas int = 1
param cpu string = '0.25'
param memory string = '0.5Gi'

// Container Image ref
param containerImage string

// Networking
param useExternalIngress bool = false
param containerPort int

param registry string
param registryUsername string
@secure()
param registryPassword string
var registryPasswordName = 'container-registry-password'

param domainName string
param certificateName string
@secure()
param certificateValue string

param envVars array = []

resource containerAppEnvironment 'Microsoft.App/managedEnvironments@2022-06-01-preview' existing = {
  name: containerAppEnvironmentName
}

resource certificate 'Microsoft.App/managedEnvironments/certificates@2022-06-01-preview' = {
  parent: containerAppEnvironment
  name: certificateName
  location: location
  properties: {
    value: certificateValue
  }
}

resource containerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: name
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    managedEnvironmentId: containerAppEnvironment.id
    configuration: {
      secrets: [
        {
          name: registryPasswordName
          value: registryPassword
        }
      ]
      registries: [
        {
          server: registry
          username: registryUsername
          passwordSecretRef: registryPasswordName
        }
      ]
      ingress: {
        allowInsecure: false
        external: useExternalIngress
        targetPort: containerPort
        customDomains: [
          {
            name: domainName
            certificateId: certificate.id
          }
        ]
      }
    }
    template: {
      containers: [
        {
          image: containerImage
          name: name
          env: envVars
          resources: {
            cpu: cpu
            memory: memory
          }
          probes: [
            {
              type: 'Startup'
              httpGet: {
                path: '/healthz'
                port: 80
                scheme: 'HTTP'
              }
              initialDelaySeconds: 60
              periodSeconds: 12
              failureThreshold: 10
            }
            {
              type: 'Liveness'
              httpGet: {
                path: '/healthz'
                port: 80
                scheme: 'HTTP'
              }
              periodSeconds: 30
              failureThreshold: 10
            }
          ]
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
      }
    }
  }
}

output fqdn string = containerApp.properties.configuration.ingress.fqdn
output appId string = containerApp.id
output appPrincipalId string = containerApp.identity.principalId
