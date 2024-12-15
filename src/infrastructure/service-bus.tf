resource "azurerm_servicebus_namespace" "namespace" {
  name = "sb-demo-namespace"
  resource_group_name = azurerm_resource_group.rg.name
  location = azurerm_resource_group.rg.location
  sku = "Standard"
}

resource "azurerm_servicebus_queue" "queue" {
  name = "teste-matheus"
  namespace_id = azurerm_servicebus_namespace.namespace.id
}