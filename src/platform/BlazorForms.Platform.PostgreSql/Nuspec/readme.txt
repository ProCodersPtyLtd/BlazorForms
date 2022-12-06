Thank you for using BlazorForms.

Installation process.

-------------------------------------------------------------------------------------------------------------------------------------------------------------
1. Install BlazorFormsServerSide nuget package to your Blazor Server App project and do all required steps from its readme.txt

2. Install BlazorForms.PostgreSql nuget package to your Blazor Server App project

3. Run PgAdmin

4. Run script from [BlazorForms.Postgres.Link\Scripts\01_CreateDatabase.sql] in your database

5. Copy configuration section from [BlazorForms.Postgres.Link\appsettings.postgres.json] to your project appsettings.json 

6. Modify your project startup.cs file and add code to ConfigureServices method right after AddServerSideBlazorForms and AddBlazorFormsApplicationParts:
            services.AddServerSideBlazorForms();
            services.AddBlazorFormsApplicationParts... 
            services.AddBlazorFormsPostgres();

Run your project
-------------------------------------------------------------------------------------------------------------------------------------------------------------

More configuration options:

7. If you want to use Cosmos DB flow storage configuration install BlazorForms.Cosmos nuget package
