# 📁 Folder Organizer — By Billah Studio

> **Universal Downloads & Folder Organizer with Modern Dark UI/UX**  
> Developed with ❤️ by **Billah Studio** ([basharbillah.com](https://www.basharbillah.com))

![License](https://img.shields.io/badge/License-MIT-blue.svg)
![Platform](https://img.shields.io/badge/Platform-Windows-0078D6.svg)
![Framework](https://img.shields.io/badge/Built%20with-.NET%20%7C%20WPF-68217A.svg)

---

## ⚡ Highlights
- 🎨 **Modern Dark UI/UX**: Sleek acrylic dark interface with glowing accents, animated progress indicators, and instant responsiveness.
- 🎯 **Target Any Folder**: Defaults to current directory, but allows browsing any folder or Drag & Drop.
- 📊 **Real-time File Scanning**: Instantly counts loose files and previews matched categories.
- 🛡️ **Safe File Moving**: Automatically prevents file overwrites by adding incremental numbers (`(1)`, `(2)`).
- 🚫 **Smart Protection**: Never moves itself, `.bat` files, or unfinished downloads (`.crdownload`, `.part`, `.tmp`).
- 🚀 **100% Standalone**: Native Windows executable — zero Python or runtime dependencies required.

---

## 📥 Downloads
- **Latest Standalone Executable:** [`Folder Organizer_ By Billah Studio.exe`](https://github.com/billahstudio/folder-organizer-billah-studio/raw/main/Folder%20Organizer_%20By%20Billah%20Studio.exe)
- **Classic Batch Script:** [`Folder Organizer_ By Billah Studio.bat`](https://github.com/billahstudio/folder-organizer-billah-studio/raw/main/Folder%20Organizer_%20By%20Billah%20Studio.bat)

---

## 🗂️ Categories Supported

| Category | File Extensions | Target Folder |
|---|---|---|
| **Adobe Creative Suite** | `.psd`, `.psb`, `.ai`, `.eps`, `.aep`, `.aet`, `.mogrt`, `.prproj`, `.indd`, `.idml`, `.sesx`, `.lrtemplate`, `.xmp`, `.xd` | `Adobe\...` |
| **Microsoft Office** | `.doc`, `.docx`, `.xls`, `.xlsx`, `.ppt`, `.pptx`, `.accdb`, `.mdb`, `.one`, `.pub`, `.vsd`, `.vsdx`, `.csv` | `MS Office\...` |
| **Documents & eBooks** | `.pdf`, `.txt`, `.log`, `.md`, `.epub`, `.mobi`, `.azw`, `.azw3`, `.djvu` | `Documents\...` |
| **Photos & RAW** | `.jpg`, `.jpeg`, `.png`, `.gif`, `.bmp`, `.webp`, `.cr2`, `.cr3`, `.nef`, `.arw`, `.dng`, `.raw`, `.svg`, `.ico` | `Photos\...` |
| **Videos** | `.mp4`, `.mkv`, `.avi`, `.mov`, `.wmv`, `.flv`, `.webm`, `.3gp`, `.m4v`, `.ts`, `.mts` | `Videos\` |
| **Audio & Music** | `.mp3`, `.wav`, `.aac`, `.flac`, `.m4a`, `.ogg`, `.wma`, `.midi`, `.opus` | `Audio\` |
| **Compressed Archives** | `.zip`, `.rar`, `.7z`, `.tar`, `.gz`, `.bz2`, `.xz`, `.iso`, `.dmg` | `Compressed (Zip)\` |
| **Apps & Installers** | `.exe`, `.msi`, `.apk`, `.appx`, `.msix` | `Apps & Installers\` |
| **Fonts** | `.ttf`, `.otf`, `.woff`, `.woff2`, `.eot` | `Fonts\` |
| **3D & CAD** | `.blend`, `.obj`, `.fbx`, `.stl`, `.3ds`, `.dae`, `.dwg`, `.dxf`, `.step`, `.stp` | `3D & CAD\...` |
| **Coding & Scripts** | `.html`, `.css`, `.js`, `.ts`, `.php`, `.py`, `.java`, `.cpp`, `.c`, `.cs`, `.go`, `.rs`, `.json`, `.xml`, `.sql` | `Coding\...` |
| **Others** | All remaining uncategorized files | `Others\` |

---

## 💡 How to Build from Source
Compile using the built-in Windows C# compiler:
```powershell
& "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /target:winexe /optimize+ /codepage:65001 /win32icon:app_icon.ico /out:"Folder Organizer_ By Billah Studio.exe" /lib:"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF" /r:PresentationCore.dll,PresentationFramework.dll,WindowsBase.dll,System.Xaml.dll,System.dll,System.Core.dll,System.Windows.Forms.dll,System.Drawing.dll Program.cs
```

---

## 🌐 Credits & Support
Created with ❤️ by **Billah Studio**.  
Visit: [basharbillah.com](https://www.basharbillah.com)
