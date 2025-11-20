# Knudson Family Construction Development Server Launcher
Write-Host "Starting Knudson Family Construction..." -ForegroundColor Cyan

# Start Users API
Write-Host "`nStarting Users API (Port 5000)..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\MyUsers.Api'; dotnet run"

# Start Settings API
Write-Host "Starting Settings API (Port 5001)..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\MySettings.Api'; dotnet run"

# Wait for APIs to initialize before starting frontend
Write-Host "Waiting for APIs to start..." -ForegroundColor Gray
Start-Sleep -Seconds 8

# Start frontend dev server
Write-Host "Starting Vue Frontend..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\MyApp.Frontend'; npm run dev"

Write-Host "`n✅ Knudson Family Construction is starting!" -ForegroundColor Green
Write-Host "Users API: http://localhost:5000" -ForegroundColor Cyan
Write-Host "Settings API: http://localhost:5001" -ForegroundColor Cyan
Write-Host "Frontend: http://localhost:5173" -ForegroundColor Cyan
Write-Host "`nClose the terminal windows to stop the servers." -ForegroundColor Gray
