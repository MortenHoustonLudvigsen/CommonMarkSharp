setlocal

:: Change to this script's directory
cd /d %~dp0

set NuGet=%~dp0packages\NuGet.CommandLine.2.8.2\tools\NuGet.exe

call Pack.cmd

cd NuGetPackages

forfiles /m *.nupkg /c "cmd /c %NuGet% push @file"
