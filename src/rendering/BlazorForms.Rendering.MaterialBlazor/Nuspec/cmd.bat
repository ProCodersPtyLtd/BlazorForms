rem
cd C:\repos\BlazorForms\src\rendering\BlazorForms.Rendering.MaterialBlazor\
rem nuget.exe pack BlazorForms.Rendering.MaterialBlazor.nuspec -NonInteractive -OutputDirectory c:\temp\Nuget -Verbosity Detailed -version 0.1.304
dotnet pack BlazorForms.Rendering.MaterialBlazor.csproj --output  c:\temp\Nuget
pause