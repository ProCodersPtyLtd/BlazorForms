param location string
param tags object


///////////////////////////////////
// Resource names

param svcUserName string


///////////////////////////////////
// New resources

resource svcUser 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' = {
  name: svcUserName
  location: location
  tags: tags
}
