# Configure the Azure provider
terraform {
    backend "azurerm" {
        resource_group_name  = "tf_state_storage" #Your tf state resource group name
        storage_account_name = "tfstatestorageacctwusz14" #Your unique tf state storage account name
        container_name       = "terraform-state" #Your tf state container name
        key                  = "terraform.tfstate"
    }
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">= 2.59"
    }
    azuread = {
        version = ">= 0.7"
    }
    random = {
      version = ">= 0.0"
    }
  }
}

provider "azurerm" {
  features {}
}
resource "random_string" "acrid" {
  length = 8
  special = false
  upper = false
}
# Create a resource group
resource "azurerm_resource_group" "mainrg" {
  name     = var.resourceGroupName
  location = var.resourceRegion
}
# Create Log Analytics Workspace
resource "azurerm_log_analytics_workspace" "laworkspace" {
  name                = "acctest-01"
  location            = azurerm_resource_group.mainrg.location
  resource_group_name = azurerm_resource_group.mainrg.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}
# Create Container Insights instance
resource "azurerm_log_analytics_solution" "aksinsights" {
  solution_name         = "ContainerInsights"
  location              = azurerm_log_analytics_workspace.laworkspace.location
  resource_group_name   = azurerm_resource_group.mainrg.name
  workspace_resource_id = azurerm_log_analytics_workspace.laworkspace.id
  workspace_name        = azurerm_log_analytics_workspace.laworkspace.name

  plan {
    publisher = "Microsoft"
    product   = "OMSGallery/ContainerInsights"
  }
}
# Create Application Insights account
resource "azurerm_application_insights" "appinsights" {
  name                = "eshop-eus-appinsights"
  location            = azurerm_resource_group.mainrg.location
  resource_group_name = azurerm_resource_group.mainrg.name
  application_type    = "web"
}
# Create a Container Registry
resource "azurerm_container_registry" "acr" {
  name                = join("", [var.containerRegistryName, random_string.acrid.result])
  resource_group_name = azurerm_resource_group.mainrg.name
  location            = azurerm_resource_group.mainrg.location
  sku                 = "Standard"
  admin_enabled       = true
}
# Monitor the Container Registry
resource "azurerm_monitor_diagnostic_setting" "acrdiag" {
  name                           = "eshop-acr-eus-diag-setting"
  target_resource_id             = azurerm_container_registry.acr.id
  log_analytics_workspace_id     = azurerm_log_analytics_workspace.laworkspace.id
  log_analytics_destination_type = "Dedicated"
  log {
    category = "ContainerRegistryRepositoryEvents"
    enabled  = true
    retention_policy {
      enabled = false
    }
  }
  log {
    category = "ContainerRegistryLoginEvents"
    enabled  = true
    retention_policy {
      enabled = false
    }
  }
  metric {
    category = "AllMetrics"
    enabled  = true
    retention_policy {
      enabled = false
    }
  }
}
# Create a Service Principal and Role assignment for AKS to use with ACR
data "azuread_service_principal" "aks_principal" {
  application_id = var.client_id
}
resource "azurerm_role_assignment" "acrpull_role" {
  scope                            = azurerm_container_registry.acr.id
  role_definition_name             = "AcrPull"
  principal_id                     = data.azuread_service_principal.aks_principal.id
  skip_service_principal_aad_check = true
}
# Create a K8S cluster in Azure
resource "azurerm_kubernetes_cluster" "eshopakscluster" {
  name                = var.aksClusterName
  location            = azurerm_resource_group.mainrg.location
  resource_group_name = azurerm_resource_group.mainrg.name
  dns_prefix          = "eShopOCtada"

  default_node_pool {
    name       = "default"
    node_count = 1
    vm_size    = "Standard_D2_v2"
  }

  tags = {
    Environment = "Test"
  }

  service_principal {
    client_id     = var.client_id
    client_secret = var.client_secret
  }

  addon_profile {
    http_application_routing {
      enabled = true
    }

    oms_agent {
      enabled                    = true
      log_analytics_workspace_id = azurerm_log_analytics_workspace.laworkspace.id
    }
  }

  role_based_access_control {
    enabled = true
  }
}
# Diagnostic Settings to store AKS metrics in Log Analytics
resource "azurerm_monitor_diagnostic_setting" "aksdiag" {
  name                           = "eshop-aks-eus-diag-setting"
  target_resource_id             = azurerm_kubernetes_cluster.eshopakscluster.id
  log_analytics_workspace_id     = azurerm_log_analytics_workspace.laworkspace.id
  log_analytics_destination_type = "Dedicated"
  log {
    category = "kube-apiserver"
    enabled  = true
    retention_policy {
      enabled = false
    }
  }

  log {
    category = "kube-audit"
    enabled  = true
    retention_policy {
      enabled = false
    }
  }

  log {
    category = "cluster-autoscaler"
    enabled  = false
    retention_policy {
      enabled = false
    }
  }

  log {
    category = "kube-scheduler"
    enabled  = false
    retention_policy {
      enabled = false
    }
  }

  log {
    category = "kube-controller-manager"
    enabled  = false
    retention_policy {
      enabled = false
    }
  }

  log {
    category = "kube-apiserver"
    enabled  = true
    retention_policy {
      enabled = false
    }
  }

  metric {
    category = "AllMetrics"
    enabled  = true
    retention_policy {
      enabled = false
    }
  }
}
# Outputs
output "client_certificate" {
  value     = azurerm_kubernetes_cluster.eshopakscluster.kube_config.0.client_certificate
  sensitive = true
}

output "kube_config" {
  value     = azurerm_kubernetes_cluster.eshopakscluster.kube_config_raw
  sensitive = true
}

output "instrumentation_key" {
  value = azurerm_application_insights.appinsights.instrumentation_key
  sensitive = true
}

output "app_id" {
  value = azurerm_application_insights.appinsights.app_id
  sensitive = true
}