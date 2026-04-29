param(
   [Parameter(Mandatory = $true)]
   [string]$Path
)

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
[Console]::Write([System.IO.File]::ReadAllText($Path, [System.Text.Encoding]::Unicode))
