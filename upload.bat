@echo off
rem Copies the built mod (from the game's mods folder, where `dotnet publish` puts it)
rem into the Workshop uploader's content folder, then uploads it to Steam Workshop.

set "SRC=C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\SpireBrothers"
set "UPLOADER=D:\projects\csharp\Spire-ModUploader"
set "DEST=%UPLOADER%\SpireBrothers\content"

for %%F in (SpireBrothers.dll SpireBrothers.pck SpireBrothers.json) do (
    copy /Y "%SRC%\%%F" "%DEST%\" >nul || (
        echo Failed to copy %%F
        pause
        exit /b 1
    )
    echo Copied %%F
)

pushd "%UPLOADER%"
ModUploader.exe upload -w SpireBrothers
set "RESULT=%ERRORLEVEL%"
popd

echo.
if not "%RESULT%"=="0" echo Upload failed with code %RESULT%.
pause
exit /b %RESULT%
