```terraform
variable "subscription_id" { # Must have - Pass in using -var "subscription_id="
  type = string
  description = "contains the subscription_id for service principal"
  default   = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
}

variable "client_id" { # Must have - Pass in using -var "client_id="
  type = string
  description = "contains the Client Id for service principal"
  default   = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
}

variable "client_secret" { # Must have - Pass in using -var "client_secret="
  type = string
  description = "contains the Client Secret for service principal"
  default   = "XXXXXXXXXXXXXXXXXXXXXXXX"
}

variable "tenant_id" { # Must have - Pass in using -var "tenant_id="
  type = string
  description = "contains the Tenant Id for service principal"
  default =  "XXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
}

variable "resourceRegion" {
  type        = string
  default     = "eastus"
  description = "Location for the resource(s)."
}

variable "alertActionGroups" {
  type        = list(string)
  default     = []
  description = "Action group(s) for the alerts"
}

variable "webHookPayLoad" {
  type        = string
  default     = "{}"
  description = "Custom payload to be sent with the alert"
}
variable "containerRegistryName" {
  type        = string
  default     = "eShopeusdemoacr"
  description = "Container Registry NAme"
}
variable "resourceGroupName" {
  type        = string
  default     = "eShop-eus-demo-rg"
  description = "Resource group to contain ACR, AKS, and LA Workspace"
}
variable "aksClusterName" {
  type        = string
  default     = "eShop-eus-demo-aks"
  description = "aks cluster name"
}