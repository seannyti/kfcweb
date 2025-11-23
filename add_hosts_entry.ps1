# Add knudsongc.com to hosts file
$hostsPath = "$env:SystemRoot\System32\drivers\etc\hosts"
$entry = "23.239.26.52 knudsongc.com www.knudsongc.com"

# Check if entry already exists
$hostsContent = Get-Content $hostsPath
if ($hostsContent | Select-String "knudsongc.com") {
    Write-Host "Entry already exists in hosts file" -ForegroundColor Yellow
} else {
    # Add entry
    Add-Content -Path $hostsPath -Value "`n$entry"
    Write-Host "Successfully added entry to hosts file!" -ForegroundColor Green
}

# Display the entry
Write-Host "`nCurrent knudsongc.com entries:" -ForegroundColor Cyan
Get-Content $hostsPath | Select-String "knudsongc"

Write-Host "`nPress any key to close..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
