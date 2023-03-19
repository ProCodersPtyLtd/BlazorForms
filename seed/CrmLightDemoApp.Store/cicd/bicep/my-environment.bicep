param environment string

var environments = {
  Production:   loadJsonContent('../config-Production.json')
}

output config object = environments[environment]
