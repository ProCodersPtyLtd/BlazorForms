[Out of date]
Thank you for using BlazorForms.

Installation process.

-------------------------------------------------------------------------------------------------------------------------------------------------------------
1. Install BlazorFormsServerSide nuget package to your Blazor Server App project

    Important: You should see folders [BlazorForms.Example.Link] and [BlazorForms.Config.Link] in your project with code samples, configs and scripts

2. Modify your project startup.cs file add [using BlazorForms.Platform;] and add code to ConfigureServices method:
            services.AddServerSideBlazorForms();
            services.AddBlazorFormsApplicationParts("BlazorForms.");           // if you want to use examples from BlazorForms.Example.Link without changing namespace
            services.AddBlazorFormsApplicationParts("BlazorApp1");    // put root namespace of your project instead of BlazorApp1

    Important: Change "BlazorApp1" to your root namespace (project name). Also don't forget to add [using BlazorForms.Platform;]

3. Modify _Imports.razor and add references:
@using MatBlazor
@using BlazorForms.Rendering.Components

4. Modify _Host.cshtml and add to <head> tag before your css link
    <script src="_content/MatBlazor/dist/matBlazor.js"></script>
    <link href="_content/MatBlazor/dist/matBlazor.css" rel="stylesheet" />

5. For SqlExpress Local DB Run:  

5.1 Copy configuration sections from [BlazorForms.Config.Link\appsettings.localdb.json] to your project appsettings.json
5.2 Run Windows Command Prompt and execute commands from [BlazorForms.Config.Link\localdb.cmd] to create required sql objects

6. File [BlazorForms.Example.Link\SampleFlow1.cs] contains a sample flow with model, form and rule

7. Modify [Pages\Counter.razor] file adding razor tag:
    <DynamicForm FlowName="BlazorForms.SampleFlow1" Pk="0" IsDefaultReadonlyView="false" NavigationSuccess="/" />

Run your project
-------------------------------------------------------------------------------------------------------------------------------------------------------------

More configuration options:

8. For Microsoft SqlServer Run:
*. - ignore step 5.

8.1 Copy configuration sections from [BlazorForms.Config.Link\appsettings.mssql.json] to your project appsettings.json, modify connection strings pointing it 
    to your sql database
8.2 Run sql script [BlazorForms.Config.Link\SqlServerScripts.sql] in your sql database to create required sql objects

9. If you want to use Postgres Sql flow storage configuration install BlazorForms.Postgres nuget package

10. If you want to use CosmosDB flow storage configuration install BlazorForms.Cosmos nuget package
-------------------------------------------------------------------------------------------------------------------------------------------------------------