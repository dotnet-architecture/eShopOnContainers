#Install mesh extension
	az extension add --name mesh

#Desploy command
	az mesh deployment create --resource-group {{resource-group}} --template-file {{path-file}} --parameters {{parameters}}

	Examples:
		- [resource-group] test
		- [path-file] C:/Users/epique/Desktop/mesh_rp.linux.json
		- [parameters] "{'location': {'value': 'eastus'}}"

#Logs command
	az mesh code-package-log get --app-name {{app-name}} --code-package-name {{code-package-name}} -g {{resource-group}} --service-name {{service-name}} --replica-name {{replica-name}}

	Examples:
		- [app-name] eShopOnMesh
		- [code-package-name] catalog-api
		- [resource-group] test
		- [service-name] catalogapi-svc
		- [replica-name] 0 (is numeric, start into 0)

#Show service command
	az mesh service show --app-name {{app-name}} -g {{resource-group}} -n {{service-name}}