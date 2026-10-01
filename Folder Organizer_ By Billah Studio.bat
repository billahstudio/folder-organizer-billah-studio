@echo off
title Universal Downloads Organizer
cd /d "%~dp0"
set "self=%~nx0"

echo ==========================================================
echo          Organizing Downloads Folder... By Billah Studio
echo ==========================================================
echo.

:: ============================================================
:: 1. ADOBE CREATIVE SUITE (Folder: Adobe)
:: ============================================================
call :m "Adobe\Photoshop (PSD-PSB)" psd psb
call :m "Adobe\Illustrator (AI-EPS)" ai eps
call :m "Adobe\After Effects (AEP)" aep aet mogrt
call :m "Adobe\Premiere Pro (PRPROJ)" prproj prfpset
call :m "Adobe\InDesign (INDD)" indd idml
call :m "Adobe\Audition" sesx
call :m "Adobe\Lightroom" lrtemplate xmp
call :m "Adobe\XD" xd

:: ============================================================
:: 2. MICROSOFT OFFICE (Folder: MS Office)
:: ============================================================
call :m "MS Office\Word" doc docx docm dot dotx rtf
call :m "MS Office\Excel" xls xlsx xlsm xlsb xltx csv
call :m "MS Office\PowerPoint" ppt pptx pptm pps ppsx potx
call :m "MS Office\Access" accdb mdb
call :m "MS Office\OneNote" one
call :m "MS Office\Publisher" pub
call :m "MS Office\Visio" vsd vsdx

:: ============================================================
:: 3. DOCUMENTS & EBOOKS (Folder: Documents)
:: ============================================================
call :m "Documents\PDF" pdf
call :m "Documents\Text & Notes" txt log md
call :m "Documents\eBooks" epub mobi azw azw3 djvu

:: ============================================================
:: 4. PHOTOS & GRAPHICS (Folder: Photos)
:: ============================================================
call :m "Photos\Images" jpg jpeg png gif bmp webp
call :m "Photos\RAW Photos" cr2 cr3 nef arw dng raw tiff tif heic heif
call :m "Photos\Vector & Icons" svg ico

:: ============================================================
:: 5. VIDEO FILES (Folder: Videos)
:: ============================================================
call :m Videos mp4 mkv avi mov wmv flv webm 3gp m4v ts mts

:: ============================================================
:: 6. AUDIO & MUSIC (Folder: Audio)
:: ============================================================
call :m Audio mp3 wav aac flac m4a ogg wma midi mid opus

:: ============================================================
:: 7. COMPRESSED & ARCHIVES (Folder: Compressed)
:: ============================================================
call :m "Compressed (Zip)" zip rar 7z tar gz bz2 xz iso dmg

:: ============================================================
:: 8. APPLICATIONS & INSTALLERS (Folder: Apps & Installers)
:: ============================================================
call :m "Apps & Installers" exe msi apk appx msix

:: ============================================================
:: 9. FONTS (Folder: Fonts)
:: ============================================================
call :m Fonts ttf otf woff woff2 eot

:: ============================================================
:: 10. 3D MODELS & CAD (Folder: 3D & CAD)
:: ============================================================
call :m "3D & CAD\3D Models" blend obj fbx stl 3ds dae
call :m "3D & CAD\CAD Drawings" dwg dxf step stp

:: ============================================================
:: 11. CODING & DEVELOPMENT (Folder: Coding)
:: ============================================================
call :m "Coding\Web & Scripts" html htm css js jsx ts tsx php py java cpp c cs go rs
call :m "Coding\Data & Config" json xml yaml yml sql db sqlite

:: ============================================================
:: 12. TORRENTS & SHORTCUTS
:: ============================================================
call :m Torrents torrent
call :m Shortcuts lnk url

:: ============================================================
:: 13. REMAINING UNCATEGORIZED FILES -> Others
:: ============================================================
for %%f in (*) do (
    if /i not "%%~nxf"=="%self%" if /i not "%%~xf"==".crdownload" if /i not "%%~xf"==".part" if /i not "%%~xf"==".tmp" if /i not "%%~xf"==".downloading" (
        if not exist "Others" md "Others"
        if not exist "Others\%%~nxf" move "%%f" "Others\" >nul
    )
)

echo.
echo ============================================================
echo  Done! All files have been organized successfully! 
echo ============================================================
echo.
echo ============================================================
echo  Don't Forget to say Thanks Billah
echo ============================================================
echo.

powershell -NoProfile -Command "Add-Type -AssemblyName System.Windows.Forms; $f = New-Object Windows.Forms.Form; $f.Text = 'Thanks'; $f.Size = New-Object Drawing.Size(360, 180); $f.StartPosition = 'CenterScreen'; $f.FormBorderStyle = 'FixedDialog'; $f.MaximizeBox = $false; $f.MinimizeBox = $false; $f.TopMost = $true; $lbl = New-Object Windows.Forms.Label; $lbl.Text = 'Don''t Forget to say Thanks Billah'; $lbl.Font = New-Object Drawing.Font('Segoe UI', 10, [Drawing.FontStyle]::Bold); $lbl.TextAlign = 'MiddleCenter'; $lbl.Dock = 'Top'; $lbl.Height = 65; $btn = New-Object Windows.Forms.Button; $btn.Text = 'Thanks'; $btn.Font = New-Object Drawing.Font('Segoe UI', 10); $btn.Size = New-Object Drawing.Size(120, 36); $btn.Location = New-Object Drawing.Point(112, 75); $btn.Cursor = [Windows.Forms.Cursors]::Hand; $btn.Add_Click({ Start-Process 'https://www.basharbillah.com'; $f.Close() }); $f.Controls.Add($btn); $f.Controls.Add($lbl); $f.AcceptButton = $btn; [void]$f.ShowDialog()"

pause
exit /b

:: ------------------------------------------------------
:: Helper Subroutine to organize files by extension
:: ------------------------------------------------------
:m
set "d=%~1"
:n
shift
if "%~1"=="" exit /b
if exist "*.%~1" (
    if not exist "%d%" md "%d%"
    for %%f in ("*.%~1") do (
        if /i not "%%~nxf"=="%self%" (
            if not exist "%d%\%%~nxf" move "%%f" "%d%\" >nul
        )
    )
)
goto n
