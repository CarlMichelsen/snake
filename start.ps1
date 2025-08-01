# start-projects.ps1
$dotnetProjects = @(
    ".\Api\Api.csproj"
    # "..\Identity\App\App.csproj"
)

$processes = @()

try {
    # Start .NET projects
    foreach ($project in $dotnetProjects) {
        $proc = Start-Process -PassThru -FilePath "dotnet" -ArgumentList "run", "--project", $project
        $processes += $proc
        Write-Host "Started $project (PID: $($proc.Id))"
    }

    # Start frontend
    $frontendProc = Start-Process -PassThru -FilePath "cmd" -ArgumentList "/c", "npm run dev" -WorkingDirectory ".\Frontend"
    $processes += $frontendProc
    Write-Host "Started Frontend (PID: $($frontendProc.Id))"

    Write-Host "All projects started. Press Ctrl+C to stop..."

    while ($true) {
        Start-Sleep 1
    }
}
finally {
    Write-Host "Stopping all projects..."

    # Kill process trees (including child processes)
    $processes | ForEach-Object {
        if (!$_.HasExited) {
            try {
                # Kill the process tree using taskkill
                & taskkill /F /T /PID $_.Id 2>$null
            } catch {
                $_.Kill()
            }
        }
    }

    # Additional cleanup for any remaining npm/node processes
    Get-Process -Name "node" -ErrorAction SilentlyContinue | Stop-Process -Force
}