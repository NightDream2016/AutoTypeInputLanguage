@echo off
cd %~dp0

echo Deleting existing PlgX folder
rmdir /s /q PlgX

echo Creating PlgX folder
mkdir PlgX

echo Copying files
xcopy "AutoTypeInputLanguage" PlgX /s /e /exclude:PlgXExclude.txt

echo Compiling PlgX
"AutoTypeInputLanguage/bin/Release/KeePass.exe" /plgx-create "%~dp0PlgX"

echo Releasing PlgX
move /y PlgX.plgx "./AutoTypeInputLanguage.plgx"

echo Cleaning up
rmdir /s /q PlgX
