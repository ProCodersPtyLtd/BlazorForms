rem

cd C:\repos\BlazorForms\src\platform\BlazorForms.Platform\Nuspec
nuget.exe pack BlazorForms.nuspec -NonInteractive -OutputDirectory c:\temp\Nuget -Verbosity Detailed -version 1.12.1
nuget.exe pack BlazorForms.Shared.nuspec -NonInteractive -OutputDirectory c:\temp\Nuget -Verbosity Detailed -version 1.12.0
nuget.exe pack BlazorForms.Rendering.nuspec -NonInteractive -OutputDirectory c:\temp\Nuget -Verbosity Detailed -version 1.12.0
nuget.exe pack BlazorForms.Rendering.Flows.nuspec -NonInteractive -OutputDirectory c:\temp\Nuget -Verbosity Detailed -version 1.12.0
nuget.exe pack BlazorForms.Cosmos.nuspec -NonInteractive -OutputDirectory c:\temp\Nuget -Verbosity Detailed -version 1.12.1

rem cd C:\repos\BlazorForms\src\rendering\BlazorForms.Rendering.Flows\
rem dotnet pack BlazorForms.Rendering.Flows.csproj --output  c:\temp\Nuget

cd C:\repos\BlazorForms\src\rendering\BlazorForms.Rendering.MudBlazorUI\
dotnet pack BlazorForms.Rendering.MudBlazorUI.csproj --output  c:\temp\Nuget

pause