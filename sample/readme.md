# How to setup the configurations for sample projects

**Navigate to the project folder and run the following commands to set up the user secrets.**

```console
dotnet user-secrets init
dotnet user-secrets set "AzureOpenAI:ChatDeploymentName" "YOUR_DEPLOYMENT_NAME"
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://YOUR_ENDPOINT.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:ApiKey" "YOUR_API_KEY"
```
