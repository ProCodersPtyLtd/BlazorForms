Thank you for using BlazorForms.

Installation process.

-------------------------------------------------------------------------------------------------------------------------------------------------------------
1. Install BlazorFormsServerSide nuget package to your Blazor Application Server project and follow instructions from its readme.txt

2. Install BlazorForms.WebAssembly nuget package to your Blazor Apllication Client project

3. Install BlazorForms.WebAssembly nuget package to your Blazor Shared project (it needs to see BlazorForms libraries too)

4. Copy WasmSampleModel1.Shared code from Blazor Apllication Client folder [BlazorForms.Example.Link] to your Shared assembly
(if you want to copy a code file - you will need to set [Build Action: C# compiler] on it in File Properties window)

5. Copy WasmSampleFlow1.Server code from Blazor Apllication Client folder [BlazorForms.Example.Link] to your Server assembly
(if you want to copy a code file - you will need to set [Build Action: C# compiler] on it in File Properties window)

6. Modify your project Program.cs or startup.cs file add [using BlazorForms;] and add code to reigster services:
            builder.Services.AddBlazorFormsWebAssembly();
            builder.Services.AddBlazorFormsModelAssemblyTypes(typeof(Program));       // types in Blazor Apllication Client assembly (if you have models there)
            builder.Services.AddBlazorFormsModelAssemblyTypes(typeof(WasmSampleModel1));      // model types from assembly you share with Blazor Apllication Server 

    Important: It may complain about versions of libraries (Microsoft.Extensions.DependencyInjection.Abstractions, Microsoft.AspNetCore.Components, etc.) please 
                install latest versions using Manage NuGet Packages...

7. Modify _Imports.razor and add references:
@using MatBlazor
@using BlazorForms.Rendering.Components

8. Modify index.html page and add to <head> tag before your css link
    <script src="_content/MatBlazor/dist/matBlazor.js"></script>
    <link href="_content/MatBlazor/dist/matBlazor.css" rel="stylesheet" />

9. Modify [Pages\Counter.razor] file adding razor tag:
    <DynamicForm FlowName="BlazorForms.WasmSampleFlow1" Pk="0" IsDefaultReadonlyView="false" NavigationSuccess="/" />

10. In Server project modify [startup.cs] adding json compatible settings:
            services.AddControllersWithViews().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

Run your Blazor Application Server project
-------------------------------------------------------------------------------------------------------------------------------------------------------------
