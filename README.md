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
