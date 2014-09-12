setlocal

:: Change to this script's directory
cd /d %~dp0

set NuGet=%~dp0packages\NuGet.CommandLine.2.8.2\tools\NuGet.exe

rmdir /Q /S NuGetPackages
mkdir NuGetPackages
%NuGet% pack CommonMarkSharp\CommonMarkSharp.csproj -Build -Properties "Configuration=Release" -Symbols -OutputDirectory NuGetPackages
