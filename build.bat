@echo off

:: Set Visual Studio environment variables
set VSDIR=C:\Program Files (x86)\Microsoft Visual Studio 14.0
set VSVARS=%VSDIR%\Common7\Tools\vsvars32.bat
call "%VSVARS%"

pushd %~dp0

:: Create directory for build results
set BUILDDIR=Build\OculusTray %date% %time%
set BUILDDIR=%BUILDDIR::=-%
set BUILDDIR=%BUILDDIR:.=-%
echo Copying build results to directory: %BUILDDIR%
mkdir "%BUILDDIR%"

:: Build applications
call :BuildProject OculusTray

:: Pack build results into a zip file
7za a -r "%BUILDDIR%.zip" ".\%BUILDDIR%\*"

popd
pause
exit /b %errorlevel%

:BuildProject
set PROJECT=%~1

msbuild %PROJECT%.csproj /t:Rebuild /p:Configuration=Release
robocopy bin\Release "%BUILDDIR%" *.exe *.dll *.txt *.md
