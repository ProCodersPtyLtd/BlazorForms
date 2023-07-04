///////////////////////////////////
// Resource names
param conrtibutorPrincipalId string
param svcStorageAccountName string

@description('This is the built-in Storage Blob Data Contributor role. See https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#storage-blob-data-contributor ')
resource storageBlobDataContributorRoleDefinition 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  scope: subscription()
  name: 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
}

@description('This is the built-in Storage Queue Data Contributor role. See https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#storage-queue-data-contributor ')
resource atorageQueueDataContributorRoleDefinition 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  scope: subscription()
  name: '974c5e8b-45b9-4653-ba55-5f855dd0fb88'
}

///////////////////////////////////
// New resources

resource svcStorageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' existing = {
  name: svcStorageAccountName
}


@description('Allows the service user to manage the Data Protection keys and any other blobs the service might require')
resource svcUserblobContributer 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid('svcUserBlobContributor', svcStorageAccount.id, conrtibutorPrincipalId)
  scope: svcStorageAccount
  properties: {
    roleDefinitionId: storageBlobDataContributorRoleDefinition.id
    principalId: conrtibutorPrincipalId
    principalType: 'ServicePrincipal'
  }
}

@description('Allows the service user to manage the Data Protection keys and any other blobs the service might require')
resource svcUserblobQueueContributor 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid('svcUserblobQueueContributor', svcStorageAccount.id, conrtibutorPrincipalId)
  scope: svcStorageAccount
  properties: {
    roleDefinitionId: atorageQueueDataContributorRoleDefinition.id
    principalId: conrtibutorPrincipalId
    principalType: 'ServicePrincipal'
  }
}
