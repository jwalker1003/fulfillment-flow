# Stop / SubagentStop hook.
# Before Claude finishes, require `dotnet build -warnaserror` and `dotnet test` to pass
# whenever source has changed since the last green run.
#  - Skips when there is no solution yet (before story P1-01).
#  - Skips when HEAD + uncommitted source changes match the last green fingerprint,
#    so plain conversation costs nothing.
#  - Blocks (exit 2, output sent back to Claude) at most 3 times in a row, then lets
#    Claude stop and warns the user, so it can never loop forever.

$ErrorActionPreference = 'Continue'
$maxBlocks = 3

$projectDir = $env:CLAUDE_PROJECT_DIR
if ([string]::IsNullOrWhiteSpace($projectDir)) { $projectDir = (Resolve-Path "$PSScriptRoot\..\..").Path }
Set-Location $projectDir

# Drain stdin (payload unused).
try { [void][Console]::In.ReadToEnd() } catch { }

if (-not (Get-ChildItem -Path $projectDir -Filter '*.sln' -File -ErrorAction SilentlyContinue)) { exit 0 }

$stateDir     = Join-Path $projectDir '.claude\hooks'
$greenFile    = Join-Path $stateDir '.last-green'
$attemptsFile = Join-Path $stateDir '.stop-attempts'

$head   = (& git rev-parse HEAD 2>$null) -join ''
$dirty  = (& git status --porcelain -- src tests '*.sln' 'Directory.Build.props' 'Directory.Packages.props' 'global.json' 2>$null) -join "`n"
$fingerprint = "$head|$dirty"

if ((Test-Path $greenFile) -and ((Get-Content $greenFile -Raw) -eq $fingerprint)) { exit 0 }

$attempts = 0
if (Test-Path $attemptsFile) { [int]::TryParse((Get-Content $attemptsFile -Raw).Trim(), [ref]$attempts) | Out-Null }
if ($attempts -ge $maxBlocks) {
    Set-Content -Path $attemptsFile -Value '0' -NoNewline
    @{ systemMessage = "verify-build: build or tests still failing after $maxBlocks attempts. Claude was allowed to stop. Check the output before continuing." } | ConvertTo-Json -Compress
    exit 0
}

function Fail([string]$stage, [string[]]$output) {
    Set-Content -Path $attemptsFile -Value ([string]($attempts + 1)) -NoNewline
    $tail = ($output | Select-Object -Last 60) -join "`n"
    [Console]::Error.WriteLine("verify-build: '$stage' failed. Fix it before finishing (attempt $($attempts + 1) of $maxBlocks).`n$tail")
    exit 2
}

$build = & dotnet build -warnaserror -nologo -v q 2>&1
if ($LASTEXITCODE -ne 0) { Fail 'dotnet build -warnaserror' $build }

$test = & dotnet test --no-build -nologo -v q 2>&1
if ($LASTEXITCODE -ne 0) { Fail 'dotnet test' $test }

Set-Content -Path $greenFile -Value $fingerprint -NoNewline
Set-Content -Path $attemptsFile -Value '0' -NoNewline
exit 0
