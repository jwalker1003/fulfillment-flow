# PreToolUse hook (Edit|Write|MultiEdit|NotebookEdit).
# While a factory run is active (.claude/factory-run.lock exists), deny edits to
# the authoritative requirements, the original spec, and the ADRs.
# Outside a run, Brad (or Claude at his request) can edit them normally.

$ErrorActionPreference = 'Stop'
try {
    $raw = [Console]::In.ReadToEnd()
    if ([string]::IsNullOrWhiteSpace($raw)) { exit 0 }
    $payload = $raw | ConvertFrom-Json

    $projectDir = $env:CLAUDE_PROJECT_DIR
    if ([string]::IsNullOrWhiteSpace($projectDir)) { $projectDir = (Resolve-Path "$PSScriptRoot\..\..").Path }
    if (-not (Test-Path (Join-Path $projectDir '.claude\factory-run.lock'))) { exit 0 }

    $path = $payload.tool_input.file_path
    if ([string]::IsNullOrWhiteSpace($path)) { $path = $payload.tool_input.notebook_path }
    if ([string]::IsNullOrWhiteSpace($path)) { exit 0 }

    $normalized = $path -replace '\\', '/'
    $protected = @(
        'docs/01-product/requirements\.md$',
        'docs/03-architecture/adr/',
        'Requierments and AC Specs\.md$'
    )
    foreach ($pattern in $protected) {
        if ($normalized -match $pattern) {
            $result = @{
                hookSpecificOutput = @{
                    hookEventName            = 'PreToolUse'
                    permissionDecision       = 'deny'
                    permissionDecisionReason = "Protected during factory runs: $path. Requirements and ADRs change only with Brad's approval. Record the conflict in the story's Notes in progress.md (or report BLOCKED) instead."
                }
            }
            $result | ConvertTo-Json -Depth 5 -Compress
            exit 0
        }
    }
    exit 0
}
catch {
    # Never break editing because of a hook bug; report it to the user.
    @{ systemMessage = "protect-docs hook error: $($_.Exception.Message)" } | ConvertTo-Json -Compress
    exit 0
}
