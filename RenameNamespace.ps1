Get-ChildItem -Path . -Recurse -File -Include *.cs, *.xaml | ForEach-Object {
    $currentFile = $_

    if ($currentFile.Extension -notin (".baml", ".g.cs", ".g.i.cs")) {
        $content = Get-Content $currentFile -Raw
        $updatedContent = $content `
            -replace 'ManagerTest', 'Healthmanagment' `
            -replace 'MangerTest', 'Healthmanagment'  # <- f�r Tippfehler

        if ($content -ne $updatedContent) {
            Set-Content $currentFile $updatedContent
            Write-Host "? Ge�ndert: $($currentFile.FullName)"
        } else {
            Write-Host "? Keine �nderung: $($currentFile.FullName)"
        }
    }
}
