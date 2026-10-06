@echo off
rem Renders every frame for all four brothers into the "out" folder next to this file, then opens it.
cd /d "%~dp0"
"C:\Program Files\dotnet\dotnet.exe" run -c Release -- out
if errorlevel 1 (
  echo.
  echo Something went wrong. The error is above.
  pause
  exit /b 1
)
start "" "%~dp0out"
