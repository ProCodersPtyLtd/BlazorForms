cd C:\repos\BlazorForms\src\platform\BlazorForms.Platform\Nuspec
nuget.exe pack BlazorForms.nuspec -NonInteractive -OutputDirectory c:\temp\Nuget -Verbosity Detailed -version 0.8.1
nuget.exe pack BlazorForms.Rendering.nuspec -NonInteractive -OutputDirectory c:\temp\Nuget -Verbosity Detailed -version 0.8.0

cd C:\repos\BlazorForms\src\rendering\BlazorForms.Rendering.MudBlazorUI\
dotnet pack BlazorForms.Rendering.MudBlazorUI.csproj --output  c:\temp\Nuget

pause