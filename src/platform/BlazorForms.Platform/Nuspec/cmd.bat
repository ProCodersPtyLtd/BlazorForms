cd C:\repos\BlazorForms\src\platform\BlazorForms.Platform\Nuspec
nuget.exe pack BlazorForms.nuspec -NonInteractive -OutputDirectory c:\temp\Nuget -Verbosity Detailed -version 0.3.5
nuget.exe pack BlazorForms.Rendering.nuspec -NonInteractive -OutputDirectory c:\temp\Nuget -Verbosity Detailed -version 0.3.5
pause