Thank you for using BlazorForms.

Installation process.

-------------------------------------------------------------------------------------------------------------------------------------------------------------
1. Install BlazorFormsServerSide nuget package to your Blazor Server App project and do all required steps from its readme.txt

2. Install BlazorForms.Cosmos nuget package to your Blazor Server App project

3. Run Azure Cosmos DB Emulator

4. Copy configuration section from [BlazorForms.Cosmos.Link\appsettings.cosmos.json] to your project appsettings.json 

5. Modify your project startup.cs file and add code to ConfigureServices method right after AddServerSideBlazorForms and AddBlazorFormsApplicationParts:
            services.AddServerSideBlazorForms();
            services.AddBlazorFormsApplicationParts... 
            services.AddBlazorFormsCosmos();

Run your project
-------------------------------------------------------------------------------------------------------------------------------------------------------------

More configuration options:

6. Modify "CosmosDb" section in your appsettings.json pointing it to your Azure Cosmos DB

7. If you want to use Postgres SQL flow storage configuration install BlazorForms.Postgres nuget package
