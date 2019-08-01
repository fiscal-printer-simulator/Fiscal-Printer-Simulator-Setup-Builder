cd %~dp0
Powershell -Command CI\CakeScripts\build.ps1 -Configuration=Release -TargetPlatform=x86
PAUSE