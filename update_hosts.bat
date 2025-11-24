@echo off
echo Updating hosts file...
powershell -Command "$hostsPath = 'C:\Windows\System32\drivers\etc\hosts'; $content = Get-Content $hostsPath; $newContent = $content | Where-Object { $_ -notmatch 'knudsongc\.com' }; $newContent += ''; $newContent += '23.239.26.52 knudsonconstruction.com www.knudsonconstruction.com'; Set-Content -Path $hostsPath -Value $newContent -Force"
echo.
echo Done! Hosts file updated.
echo Old domain (knudsongc.com) removed.
echo New domain (knudsonconstruction.com) added.
pause
