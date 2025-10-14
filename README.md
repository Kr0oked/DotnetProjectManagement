# Reference Application - "Project Management"

A reference .NET application implementing a project management software.

Focused on:

* Clean Architecture
* Clean Code
* .NET Aspire
* Blazor WebAssembly
* Entity Framework Core
* Keycloak
* SignalR

## Create migration

 ```
dotnet ef migrations add MIGRATION_NAME --project src/ProjectManagement.Migrations --startup-project src/ProjectManagement.Migrations.Job
 ```

## Kiota generation

```
dotnet kiota generate --openapi src/AppHost/external-resource/openapi.yaml --language CSharp --output src/ExternalResource/Client --class-name ExternalResourceClient --namespace-name DotnetProjectManagement.ExternalResource.Client
```
