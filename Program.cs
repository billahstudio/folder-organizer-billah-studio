using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace FolderOrganizerByBillahStudio
{
    public class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AttachConsole(int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        private const int ATTACH_PARENT_PROCESS = -1;

        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    MessageBox.Show("Fatal Error: " + (e.ExceptionObject != null ? e.ExceptionObject.ToString() : "Unknown"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                };

                if (args != null && args.Length > 0 && args[0].Equals("--cli", StringComparison.OrdinalIgnoreCase))
                {
                    AttachConsole(ATTACH_PARENT_PROCESS);
                    string target = args.Length > 1 ? args[1] : AppDomain.CurrentDomain.BaseDirectory;
                    Console.WriteLine("==========================================================");
                    Console.WriteLine("  Folder Organizer CLI Mode — By Billah Studio");
                    Console.WriteLine("==========================================================");
                    Console.WriteLine("Target: " + target);
                    
                    var engine = new OrganizationEngine();
                    int moved = engine.OrganizeFolder(target, true, (msg, type) =>
                    {
                        Console.WriteLine(string.Format("[{0}] {1}", type.ToUpper(), msg));
                    });

                    Console.WriteLine("==========================================================");
                    Console.WriteLine(string.Format("Done! Organized {0} files. Say Thanks Billah!", moved));
                    Console.WriteLine("==========================================================");
                    FreeConsole();
                    return;
                }

                var app = new Application();
                app.DispatcherUnhandledException += (s, e) =>
                {
                    MessageBox.Show("Application Error: " + e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    e.Handled = true;
                };

                var mainWin = new MainWindow();
                app.Run(mainWin);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Startup Error: " + ex.Message + "\n" + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class RuleItem
    {
        public string RelativeSubfolder { get; set; }
        public HashSet<string> Extensions { get; set; }

        public RuleItem(string folder, string[] exts)
        {
            RelativeSubfolder = folder;
            Extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < exts.Length; i++)
            {
                Extensions.Add(exts[i].TrimStart('.'));
            }
        }
    }

    public class OrganizationEngine
    {
        public List<RuleItem> Rules { get; private set; }
        public HashSet<string> TempExtensions { get; private set; }

        public OrganizationEngine()
        {
            Rules = new List<RuleItem>
            {
                // 1. Adobe Creative Suite
                new RuleItem(@"Adobe\Photoshop (PSD-PSB)", new[] { "psd", "psb" }),
                new RuleItem(@"Adobe\Illustrator (AI-EPS)", new[] { "ai", "eps" }),
                new RuleItem(@"Adobe\After Effects (AEP)", new[] { "aep", "aet", "mogrt" }),
                new RuleItem(@"Adobe\Premiere Pro (PRPROJ)", new[] { "prproj", "prfpset" }),
                new RuleItem(@"Adobe\InDesign (INDD)", new[] { "indd", "idml" }),
                new RuleItem(@"Adobe\Audition", new[] { "sesx" }),
                new RuleItem(@"Adobe\Lightroom", new[] { "lrtemplate", "xmp" }),
                new RuleItem(@"Adobe\XD", new[] { "xd" }),

                // 2. Microsoft Office
                new RuleItem(@"MS Office\Word", new[] { "doc", "docx", "docm", "dot", "dotx", "rtf" }),
                new RuleItem(@"MS Office\Excel", new[] { "xls", "xlsx", "xlsm", "xlsb", "xltx", "csv" }),
                new RuleItem(@"MS Office\PowerPoint", new[] { "ppt", "pptx", "pptm", "pps", "ppsx", "potx" }),
                new RuleItem(@"MS Office\Access", new[] { "accdb", "mdb" }),
                new RuleItem(@"MS Office\OneNote", new[] { "one" }),
                new RuleItem(@"MS Office\Publisher", new[] { "pub" }),
                new RuleItem(@"MS Office\Visio", new[] { "vsd", "vsdx" }),

                // 3. Documents & eBooks
                new RuleItem(@"Documents\PDF", new[] { "pdf" }),
                new RuleItem(@"Documents\Text & Notes", new[] { "txt", "log", "md" }),
                new RuleItem(@"Documents\eBooks", new[] { "epub", "mobi", "azw", "azw3", "djvu" }),

                // 4. Photos & Graphics
                new RuleItem(@"Photos\Images", new[] { "jpg", "jpeg", "png", "gif", "bmp", "webp" }),
                new RuleItem(@"Photos\RAW Photos", new[] { "cr2", "cr3", "nef", "arw", "dng", "raw", "tiff", "tif", "heic", "heif" }),
                new RuleItem(@"Photos\Vector & Icons", new[] { "svg", "ico" }),

                // 5. Video Files
                new RuleItem(@"Videos", new[] { "mp4", "mkv", "avi", "mov", "wmv", "flv", "webm", "3gp", "m4v", "ts", "mts" }),

                // 6. Audio & Music
                new RuleItem(@"Audio", new[] { "mp3", "wav", "aac", "flac", "m4a", "ogg", "wma", "midi", "mid", "opus" }),

                // 7. Compressed & Archives
                new RuleItem(@"Compressed (Zip)", new[] { "zip", "rar", "7z", "tar", "gz", "bz2", "xz", "iso", "dmg" }),

                // 8. Applications & Installers
                new RuleItem(@"Apps & Installers", new[] { "exe", "msi", "apk", "appx", "msix" }),

                // 9. Fonts
                new RuleItem(@"Fonts", new[] { "ttf", "otf", "woff", "woff2", "eot" }),

                // 10. 3D Models & CAD
                new RuleItem(@"3D & CAD\3D Models", new[] { "blend", "obj", "fbx", "stl", "3ds", "dae" }),
                new RuleItem(@"3D & CAD\CAD Drawings", new[] { "dwg", "dxf", "step", "stp" }),

                // 11. Coding & Development
                new RuleItem(@"Coding\Web & Scripts", new[] { "html", "htm", "css", "js", "jsx", "ts", "tsx", "php", "py", "java", "cpp", "c", "cs", "go", "rs" }),
                new RuleItem(@"Coding\Data & Config", new[] { "json", "xml", "yaml", "yml", "sql", "db", "sqlite" }),

                // 12. Torrents & Shortcuts
                new RuleItem(@"Torrents", new[] { "torrent" }),
                new RuleItem(@"Shortcuts", new[] { "lnk", "url" })
            };

            TempExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "crdownload", "part", "tmp", "downloading"
            };
        }

        public string FindCategory(string ext)
        {
            if (string.IsNullOrEmpty(ext)) return null;
            for (int i = 0; i < Rules.Count; i++)
            {
                if (Rules[i].Extensions.Contains(ext))
                {
                    return Rules[i].RelativeSubfolder;
                }
            }
            return null;
        }

        public string GetSafeDestinationPath(string destPath)
        {
            if (!File.Exists(destPath)) return destPath;
            string dir = Path.GetDirectoryName(destPath);
            string name = Path.GetFileNameWithoutExtension(destPath);
            string ext = Path.GetExtension(destPath);
            int count = 1;
            while (true)
            {
                string candidate = Path.Combine(dir, string.Format("{0} ({1}){2}", name, count, ext));
                if (!File.Exists(candidate)) return candidate;
                count++;
            }
        }

        public int OrganizeFolder(string targetDir, bool moveOthers, Action<string, string> logCallback)
        {
            if (!Directory.Exists(targetDir))
            {
                if (logCallback != null) logCallback("Target folder does not exist: " + targetDir, "error");
                return 0;
            }

            string currentExe = Process.GetCurrentProcess().MainModule.ModuleName;
            string[] files = Directory.GetFiles(targetDir);
            int moved = 0;

            for (int i = 0; i < files.Length; i++)
            {
                string f = files[i];
                string fname = Path.GetFileName(f);
                string ext = Path.GetExtension(f).TrimStart('.');

                if (string.Equals(fname, currentExe, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(fname, "Folder Organizer_ By Billah Studio.bat", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(fname, "Folder Organizer_ By Billah Studio.exe", StringComparison.OrdinalIgnoreCase) ||
                    TempExtensions.Contains(ext))
                {
                    if (logCallback != null) logCallback(fname + " (Protected/Temporary)", "skip");
                    continue;
                }

                string cat = FindCategory(ext);
                if (string.IsNullOrEmpty(cat))
                {
                    if (moveOthers)
                    {
                        cat = "Others";
                    }
                    else
                    {
                        if (logCallback != null) logCallback(fname + " (Uncategorized)", "skip");
                        continue;
                    }
                }

                string destDir = Path.Combine(targetDir, cat);
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                string targetPath = Path.Combine(destDir, fname);
                targetPath = GetSafeDestinationPath(targetPath);

                try
                {
                    File.Move(f, targetPath);
                    moved++;
                    if (logCallback != null) logCallback(string.Format("{0} ➔ {1}", Path.GetFileName(targetPath), cat), "moved");
                }
                catch (Exception ex)
                {
                    if (logCallback != null) logCallback(fname + " failed: " + ex.Message, "error");
                }
            }

            return moved;
        }
    }

    public class MainWindow : Window
    {
        private string _targetDirectory;
        private OrganizationEngine _engine;

        // UI Controls
        private TextBox txtFolderPath;
        private TextBlock txtFileCount;
        private TextBlock txtCategoryCount;
        private TextBlock txtStatusBadge;
        private ProgressBar progressBar;
        private Paragraph logParagraph;
        private RichTextBox logBox;
        private Button btnOrganize;
        private Button btnBrowse;
        private Button btnOpenFolder;
        private Button btnRefresh;
        private Button btnClose;
        private Button btnMinimize;
        private Button btnThanksHeader;
        private Button btnThanksFooter;
        private CheckBox chkMoveOthers;
        private CheckBox chkAutoOpen;
        private Grid overlayModal;
        private Button btnModalThanks;
        private Button btnModalOpen;
        private Button btnModalClose;
        private TextBlock txtModalSummary;

        public MainWindow()
        {
            _engine = new OrganizationEngine();
            _targetDirectory = AppDomain.CurrentDomain.BaseDirectory;

            LoadUi();
            BindControls();
            RefreshScan();
        }

        private void LoadUi()
        {
            this.Title = "Folder Organizer — By Billah Studio";
            this.Width = 920;
            this.Height = 710;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
            this.Background = Brushes.Transparent;
            this.AllowDrop = true;

            string xaml = @"
<Border xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
        xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
        CornerRadius='16' BorderThickness='1.5'
        Background='#0A0F1D' BorderBrush='#1E293B'>
    <Border.Effect>
        <DropShadowEffect BlurRadius='35' Color='#000000' Opacity='0.8' ShadowDepth='6' Direction='270'/>
    </Border.Effect>
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height='64'/>
            <RowDefinition Height='*'/>
            <RowDefinition Height='46'/>
        </Grid.RowDefinitions>

        <!-- TOP BAR / TITLE BAR -->
        <Border Grid.Row='0' Background='#0F172A' CornerRadius='16,16,0,0' BorderBrush='#1E293B' BorderThickness='0,0,0,1' Name='titleBarBorder'>
            <Grid Margin='18,0,16,0'>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width='Auto'/>
                    <ColumnDefinition Width='*'/>
                    <ColumnDefinition Width='Auto'/>
                </Grid.ColumnDefinitions>

                <!-- App Icon & Title -->
                <StackPanel Grid.Column='0' Orientation='Horizontal' VerticalAlignment='Center'>
                    <Border Width='38' Height='38' CornerRadius='10' Margin='0,0,12,0'>
                        <Border.Background>
                            <LinearGradientBrush StartPoint='0,0' EndPoint='1,1'>
                                <GradientStop Color='#0284C7' Offset='0'/>
                                <GradientStop Color='#4F46E5' Offset='1'/>
                            </LinearGradientBrush>
                        </Border.Background>
                        <TextBlock Text='📁' FontSize='18' HorizontalAlignment='Center' VerticalAlignment='Center'/>
                    </Border>
                    <StackPanel VerticalAlignment='Center'>
                        <StackPanel Orientation='Horizontal'>
                            <TextBlock Text='FOLDER ORGANIZER' FontWeight='Bold' FontSize='15' Foreground='#F8FAFC' Margin='0,0,8,0'/>
                            <Border Background='#4338CA' CornerRadius='4' Padding='6,1,6,1' VerticalAlignment='Center'>
                                <TextBlock Text='DARK EDITION' FontSize='9' FontWeight='Bold' Foreground='#E0E7FF'/>
                            </Border>
                        </StackPanel>
                        <TextBlock Text='By Billah Studio  •  Smart File Management' FontSize='11' Foreground='#94A3B8' Margin='0,2,0,0'/>
                    </StackPanel>
                </StackPanel>

                <!-- Window Control Buttons -->
                <StackPanel Grid.Column='2' Orientation='Horizontal' VerticalAlignment='Center'>
                    <Button Name='btnThanksHeader' Margin='0,0,12,0' Cursor='Hand'>
                        <Button.Template>
                            <ControlTemplate TargetType='Button'>
                                <Border Name='b' Background='#1E293B' BorderBrush='#38BDF8' BorderThickness='1' CornerRadius='6' Padding='10,5,10,5'>
                                    <StackPanel Orientation='Horizontal'>
                                        <TextBlock Text='❤️ ' FontSize='12'/>
                                        <TextBlock Text='Say Thanks Billah' FontSize='11' FontWeight='SemiBold' Foreground='#38BDF8'/>
                                    </StackPanel>
                                </Border>
                                <ControlTemplate.Triggers>
                                    <Trigger Property='IsMouseOver' Value='True'>
                                        <Setter TargetName='b' Property='Background' Value='#28364D'/>
                                    </Trigger>
                                </ControlTemplate.Triggers>
                            </ControlTemplate>
                        </Button.Template>
                    </Button>

                    <Button Name='btnMinimize' Width='32' Height='32' Cursor='Hand' Margin='0,0,6,0'>
                        <Button.Template>
                            <ControlTemplate TargetType='Button'>
                                <Border Name='b' Background='#1E293B' CornerRadius='6'>
                                    <TextBlock Text='—' FontSize='14' Foreground='#94A3B8' HorizontalAlignment='Center' VerticalAlignment='Center'/>
                                </Border>
                                <ControlTemplate.Triggers>
                                    <Trigger Property='IsMouseOver' Value='True'>
                                        <Setter TargetName='b' Property='Background' Value='#334155'/>
                                    </Trigger>
                                </ControlTemplate.Triggers>
                            </ControlTemplate>
                        </Button.Template>
                    </Button>

                    <Button Name='btnClose' Width='32' Height='32' Cursor='Hand'>
                        <Button.Template>
                            <ControlTemplate TargetType='Button'>
                                <Border Name='b' Background='#1E293B' CornerRadius='6'>
                                    <TextBlock Text='✕' FontSize='12' Foreground='#94A3B8' HorizontalAlignment='Center' VerticalAlignment='Center'/>
                                </Border>
                                <ControlTemplate.Triggers>
                                    <Trigger Property='IsMouseOver' Value='True'>
                                        <Setter TargetName='b' Property='Background' Value='#DC2626'/>
                                    </Trigger>
                                </ControlTemplate.Triggers>
                            </ControlTemplate>
                        </Button.Template>
                    </Button>
                </StackPanel>
            </Grid>
        </Border>

        <!-- MAIN CONTENT AREA -->
        <Grid Grid.Row='1' Margin='20,16,20,10'>
            <Grid.RowDefinitions>
                <RowDefinition Height='Auto'/>
                <RowDefinition Height='Auto'/>
                <RowDefinition Height='Auto'/>
                <RowDefinition Height='*'/>
                <RowDefinition Height='Auto'/>
            </Grid.RowDefinitions>

            <!-- 1. Folder Selector Card -->
            <Border Grid.Row='0' Background='#111827' CornerRadius='10' BorderBrush='#1F2937' BorderThickness='1' Padding='14' Margin='0,0,0,14'>
                <Grid>
                    <Grid.RowDefinitions>
                        <RowDefinition Height='Auto'/>
                        <RowDefinition Height='Auto'/>
                    </Grid.RowDefinitions>

                    <StackPanel Grid.Row='0' Orientation='Horizontal' Margin='0,0,0,8'>
                        <TextBlock Text='TARGET FOLDER TO ORGANIZE' FontSize='11' FontWeight='Bold' Foreground='#38BDF8'/>
                        <TextBlock Text='(Drag &amp; drop any folder here or click Browse)' FontSize='11' Foreground='#64748B' Margin='8,0,0,0'/>
                    </StackPanel>

                    <Grid Grid.Row='1'>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width='*'/>
                            <ColumnDefinition Width='Auto'/>
                            <ColumnDefinition Width='Auto'/>
                            <ColumnDefinition Width='Auto'/>
                        </Grid.ColumnDefinitions>

                        <Border Grid.Column='0' Background='#090D16' CornerRadius='6' BorderBrush='#334155' BorderThickness='1' Padding='8,6,8,6' Margin='0,0,8,0'>
                            <TextBox Name='txtFolderPath' Background='Transparent' BorderThickness='0' Foreground='#F1F5F9' FontSize='12' IsReadOnly='True' VerticalAlignment='Center'/>
                        </Border>

                        <Button Name='btnBrowse' Grid.Column='1' Margin='0,0,6,0' Cursor='Hand'>
                            <Button.Template>
                                <ControlTemplate TargetType='Button'>
                                    <Border Name='b' Background='#1E293B' BorderBrush='#475569' BorderThickness='1' CornerRadius='6' Padding='11,6,11,6'>
                                        <TextBlock Text='📂 Browse...' Foreground='#F8FAFC' FontSize='12' FontWeight='Medium'/>
                                    </Border>
                                    <ControlTemplate.Triggers>
                                        <Trigger Property='IsMouseOver' Value='True'>
                                            <Setter TargetName='b' Property='Background' Value='#334155'/>
                                        </Trigger>
                                    </ControlTemplate.Triggers>
                                </ControlTemplate>
                            </Button.Template>
                        </Button>

                        <Button Name='btnOpenFolder' Grid.Column='2' Margin='0,0,6,0' Cursor='Hand'>
                            <Button.Template>
                                <ControlTemplate TargetType='Button'>
                                    <Border Name='b' Background='#1E293B' BorderBrush='#475569' BorderThickness='1' CornerRadius='6' Padding='11,6,11,6'>
                                        <TextBlock Text='📁 Open' Foreground='#F8FAFC' FontSize='12' FontWeight='Medium'/>
                                    </Border>
                                    <ControlTemplate.Triggers>
                                        <Trigger Property='IsMouseOver' Value='True'>
                                            <Setter TargetName='b' Property='Background' Value='#334155'/>
                                        </Trigger>
                                    </ControlTemplate.Triggers>
                                </ControlTemplate>
                            </Button.Template>
                        </Button>

                        <Button Name='btnRefresh' Grid.Column='3' Cursor='Hand'>
                            <Button.Template>
                                <ControlTemplate TargetType='Button'>
                                    <Border Name='b' Background='#1E293B' BorderBrush='#475569' BorderThickness='1' CornerRadius='6' Padding='11,6,11,6'>
                                        <TextBlock Text='🔄 Scan' Foreground='#F8FAFC' FontSize='12' FontWeight='Medium'/>
                                    </Border>
                                    <ControlTemplate.Triggers>
                                        <Trigger Property='IsMouseOver' Value='True'>
                                            <Setter TargetName='b' Property='Background' Value='#334155'/>
                                        </Trigger>
                                    </ControlTemplate.Triggers>
                                </ControlTemplate>
                            </Button.Template>
                        </Button>
                    </Grid>
                </Grid>
            </Border>

            <!-- 2. Statistics Metric Cards -->
            <Grid Grid.Row='1' Margin='0,0,0,14'>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width='*'/>
                    <ColumnDefinition Width='*'/>
                    <ColumnDefinition Width='*'/>
                </Grid.ColumnDefinitions>

                <!-- Card 1 -->
                <Border Grid.Column='0' Background='#111827' CornerRadius='10' BorderBrush='#1F2937' BorderThickness='1' Padding='12' Margin='0,0,8,0'>
                    <StackPanel>
                        <TextBlock Text='LOOSE FILES FOUND' FontSize='10' FontWeight='Bold' Foreground='#94A3B8'/>
                        <TextBlock Name='txtFileCount' Text='0' FontSize='24' FontWeight='Bold' Foreground='#38BDF8' Margin='0,4,0,0'/>
                    </StackPanel>
                </Border>

                <!-- Card 2 -->
                <Border Grid.Column='1' Background='#111827' CornerRadius='10' BorderBrush='#1F2937' BorderThickness='1' Padding='12' Margin='0,0,8,0'>
                    <StackPanel>
                        <TextBlock Text='CATEGORIES MATCHED' FontSize='10' FontWeight='Bold' Foreground='#94A3B8'/>
                        <TextBlock Name='txtCategoryCount' Text='0' FontSize='24' FontWeight='Bold' Foreground='#A78BFA' Margin='0,4,0,0'/>
                    </StackPanel>
                </Border>

                <!-- Card 3 -->
                <Border Grid.Column='2' Background='#111827' CornerRadius='10' BorderBrush='#1F2937' BorderThickness='1' Padding='12'>
                    <StackPanel>
                        <TextBlock Text='ORGANIZER STATUS' FontSize='10' FontWeight='Bold' Foreground='#94A3B8'/>
                        <TextBlock Name='txtStatusBadge' Text='Ready to Organize' FontSize='14' FontWeight='SemiBold' Foreground='#34D399' Margin='0,9,0,0'/>
                    </StackPanel>
                </Border>
            </Grid>

            <!-- 3. Category Tags Overview -->
            <Border Grid.Row='2' Background='#0E1424' CornerRadius='8' BorderBrush='#1E293B' BorderThickness='1' Padding='10,7,10,7' Margin='0,0,0,12'>
                <WrapPanel VerticalAlignment='Center'>
                    <Border Background='#1E293B' CornerRadius='4' Padding='6,2,6,2' Margin='0,2,6,2'>
                        <TextBlock Text='🎨 Adobe Suite' FontSize='10' Foreground='#93C5FD'/>
                    </Border>
                    <Border Background='#1E293B' CornerRadius='4' Padding='6,2,6,2' Margin='0,2,6,2'>
                        <TextBlock Text='📊 MS Office' FontSize='10' Foreground='#6EE7B7'/>
                    </Border>
                    <Border Background='#1E293B' CornerRadius='4' Padding='6,2,6,2' Margin='0,2,6,2'>
                        <TextBlock Text='📑 Documents &amp; eBooks' FontSize='10' Foreground='#FDE047'/>
                    </Border>
                    <Border Background='#1E293B' CornerRadius='4' Padding='6,2,6,2' Margin='0,2,6,2'>
                        <TextBlock Text='🖼️ Photos &amp; RAW' FontSize='10' Foreground='#F472B6'/>
                    </Border>
                    <Border Background='#1E293B' CornerRadius='4' Padding='6,2,6,2' Margin='0,2,6,2'>
                        <TextBlock Text='🎬 Videos' FontSize='10' Foreground='#C084FC'/>
                    </Border>
                    <Border Background='#1E293B' CornerRadius='4' Padding='6,2,6,2' Margin='0,2,6,2'>
                        <TextBlock Text='🎵 Audio' FontSize='10' Foreground='#FB923C'/>
                    </Border>
                    <Border Background='#1E293B' CornerRadius='4' Padding='6,2,6,2' Margin='0,2,6,2'>
                        <TextBlock Text='🗜️ Compressed' FontSize='10' Foreground='#E879F9'/>
                    </Border>
                    <Border Background='#1E293B' CornerRadius='4' Padding='6,2,6,2' Margin='0,2,6,2'>
                        <TextBlock Text='⚙️ Apps' FontSize='10' Foreground='#38BDF8'/>
                    </Border>
                    <Border Background='#1E293B' CornerRadius='4' Padding='6,2,6,2' Margin='0,2,6,2'>
                        <TextBlock Text='🔤 Fonts' FontSize='10' Foreground='#CBD5E1'/>
                    </Border>
                    <Border Background='#1E293B' CornerRadius='4' Padding='6,2,6,2' Margin='0,2,6,2'>
                        <TextBlock Text='🧊 3D &amp; CAD' FontSize='10' Foreground='#A3E635'/>
                    </Border>
                    <Border Background='#1E293B' CornerRadius='4' Padding='6,2,6,2' Margin='0,2,6,2'>
                        <TextBlock Text='💻 Coding &amp; Data' FontSize='10' Foreground='#38BDF8'/>
                    </Border>
                    <Border Background='#1E293B' CornerRadius='4' Padding='6,2,6,2' Margin='0,2,6,2'>
                        <TextBlock Text='📦 Others' FontSize='10' Foreground='#94A3B8'/>
                    </Border>
                </WrapPanel>
            </Border>

            <!-- 4. Terminal Log -->
            <Border Grid.Row='3' Background='#060913' CornerRadius='10' BorderBrush='#1E293B' BorderThickness='1' Margin='0,0,0,12'>
                <Grid>
                    <Grid.RowDefinitions>
                        <RowDefinition Height='28'/>
                        <RowDefinition Height='*'/>
                    </Grid.RowDefinitions>

                    <Border Grid.Row='0' Background='#0C1222' CornerRadius='10,10,0,0' BorderBrush='#1E293B' BorderThickness='0,0,0,1' Padding='10,0,10,0'>
                        <Grid VerticalAlignment='Center'>
                            <TextBlock Text='⚡ LIVE ACTIVITY LOG' FontSize='10' FontWeight='Bold' Foreground='#64748B'/>
                            <TextBlock Text='Realtime Auto-Scroll' FontSize='9' Foreground='#475569' HorizontalAlignment='Right'/>
                        </Grid>
                    </Border>

                    <RichTextBox Name='logBox' Grid.Row='1' Background='Transparent' BorderThickness='0'
                                 Foreground='#CBD5E1' FontFamily='Consolas' FontSize='11.5' IsReadOnly='True'
                                 VerticalScrollBarVisibility='Auto' Padding='10'>
                        <FlowDocument PagePadding='0'>
                            <Paragraph Name='logParagraph'/>
                        </FlowDocument>
                    </RichTextBox>
                </Grid>
            </Border>

            <!-- 5. Controls & Action Button -->
            <Grid Grid.Row='4'>
                <Grid.RowDefinitions>
                    <RowDefinition Height='Auto'/>
                    <RowDefinition Height='Auto'/>
                    <RowDefinition Height='Auto'/>
                </Grid.RowDefinitions>

                <!-- Options -->
                <StackPanel Grid.Row='0' Orientation='Horizontal' Margin='0,0,0,8'>
                    <CheckBox Name='chkMoveOthers' IsChecked='True' VerticalAlignment='Center' Margin='0,0,20,0'>
                        <TextBlock Text='Categorize remaining unknown files into &quot;Others&quot;' Foreground='#94A3B8' FontSize='11'/>
                    </CheckBox>
                    <CheckBox Name='chkAutoOpen' IsChecked='True' VerticalAlignment='Center'>
                        <TextBlock Text='Auto open folder in Explorer upon completion' Foreground='#94A3B8' FontSize='11'/>
                    </CheckBox>
                </StackPanel>

                <!-- Progress Bar -->
                <ProgressBar Name='progressBar' Grid.Row='1' Height='8' Minimum='0' Maximum='100' Value='0' Margin='0,0,0,10'
                             Background='#111827' Foreground='#0EA5E9' BorderBrush='#1F2937' BorderThickness='1'/>

                <!-- Organize Button -->
                <Button Name='btnOrganize' Grid.Row='2' Height='46' Cursor='Hand'>
                    <Button.Template>
                        <ControlTemplate TargetType='Button'>
                            <Border Name='b' CornerRadius='8' BorderThickness='1' BorderBrush='#38BDF8'>
                                <Border.Background>
                                    <LinearGradientBrush StartPoint='0,0' EndPoint='1,0'>
                                        <GradientStop Color='#0284C7' Offset='0'/>
                                        <GradientStop Color='#4F46E5' Offset='1'/>
                                    </LinearGradientBrush>
                                </Border.Background>
                                <StackPanel Orientation='Horizontal' HorizontalAlignment='Center' VerticalAlignment='Center'>
                                    <TextBlock Text='⚡ ' FontSize='16' Foreground='White'/>
                                    <TextBlock Text='START ORGANIZING FILES' FontSize='14' FontWeight='Bold' Foreground='White'/>
                                </StackPanel>
                            </Border>
                            <ControlTemplate.Triggers>
                                <Trigger Property='IsMouseOver' Value='True'>
                                    <Setter TargetName='b' Property='Opacity' Value='0.9'/>
                                </Trigger>
                                <Trigger Property='IsPressed' Value='True'>
                                    <Setter TargetName='b' Property='Opacity' Value='0.75'/>
                                </Trigger>
                            </ControlTemplate.Triggers>
                        </ControlTemplate>
                    </Button.Template>
                </Button>
            </Grid>
        </Grid>

        <!-- FOOTER -->
        <Border Grid.Row='2' Background='#0B1120' CornerRadius='0,0,16,16' BorderBrush='#1E293B' BorderThickness='0,1,0,0' Padding='20,0,20,0'>
            <Grid VerticalAlignment='Center'>
                <StackPanel Orientation='Horizontal' VerticalAlignment='Center'>
                    <TextBlock Text='Made with ❤️ by Billah Studio' FontSize='11' Foreground='#64748B'/>
                    <TextBlock Text='  |  Universal Downloads &amp; Folder Organizer' FontSize='11' Foreground='#475569'/>
                </StackPanel>

                <Button Name='btnThanksFooter' HorizontalAlignment='Right' Cursor='Hand'>
                    <Button.Template>
                        <ControlTemplate TargetType='Button'>
                            <StackPanel Orientation='Horizontal'>
                                <TextBlock Text='🌐 ' FontSize='11'/>
                                <TextBlock Text='basharbillah.com' FontSize='11' FontWeight='SemiBold' Foreground='#38BDF8'/>
                            </StackPanel>
                        </ControlTemplate>
                    </Button.Template>
                </Button>
            </Grid>
        </Border>

        <!-- MODAL OVERLAY (Say Thanks Billah) -->
        <Grid Name='overlayModal' Grid.RowSpan='3' Background='#E5050A14' Visibility='Collapsed'>
            <Border Width='450' Height='280' Background='#0F172A' CornerRadius='14' BorderBrush='#38BDF8' BorderThickness='1.5' VerticalAlignment='Center' HorizontalAlignment='Center'>
                <Border.Effect>
                    <DropShadowEffect BlurRadius='40' Color='#0284C7' Opacity='0.5' ShadowDepth='0'/>
                </Border.Effect>
                <Grid Margin='24'>
                    <Grid.RowDefinitions>
                        <RowDefinition Height='Auto'/>
                        <RowDefinition Height='Auto'/>
                        <RowDefinition Height='Auto'/>
                        <RowDefinition Height='*'/>
                    </Grid.RowDefinitions>

                    <StackPanel Grid.Row='0' HorizontalAlignment='Center' Margin='0,0,0,10'>
                        <TextBlock Text='🎉' FontSize='32' HorizontalAlignment='Center'/>
                        <TextBlock Text='All Files Organized Successfully!' FontSize='16' FontWeight='Bold' Foreground='#F8FAFC' HorizontalAlignment='Center' Margin='0,4,0,0'/>
                    </StackPanel>

                    <TextBlock Name='txtModalSummary' Grid.Row='1' Text='45 files were sorted into their respective folders.' FontSize='12' Foreground='#94A3B8' HorizontalAlignment='Center' Margin='0,0,0,8'/>

                    <Border Grid.Row='2' Background='#1E293B' CornerRadius='8' Padding='10,6,10,6' Margin='0,0,0,16'>
                        <TextBlock Text='Don&apos;t Forget to say Thanks Billah ✨' FontSize='13' FontWeight='SemiBold' Foreground='#38BDF8' HorizontalAlignment='Center'/>
                    </Border>

                    <Grid Grid.Row='3'>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width='*'/>
                            <ColumnDefinition Width='*'/>
                            <ColumnDefinition Width='Auto'/>
                        </Grid.ColumnDefinitions>

                        <Button Name='btnModalThanks' Grid.Column='0' Margin='0,0,8,0' Height='38' Cursor='Hand'>
                            <Button.Template>
                                <ControlTemplate TargetType='Button'>
                                    <Border Background='#0284C7' CornerRadius='6'>
                                        <TextBlock Text='❤️ Say Thanks' Foreground='White' FontWeight='Bold' FontSize='12' HorizontalAlignment='Center' VerticalAlignment='Center'/>
                                    </Border>
                                </ControlTemplate>
                            </Button.Template>
                        </Button>

                        <Button Name='btnModalOpen' Grid.Column='1' Margin='0,0,8,0' Height='38' Cursor='Hand'>
                            <Button.Template>
                                <ControlTemplate TargetType='Button'>
                                    <Border Background='#334155' CornerRadius='6'>
                                        <TextBlock Text='📁 Open Folder' Foreground='#F8FAFC' FontWeight='SemiBold' FontSize='12' HorizontalAlignment='Center' VerticalAlignment='Center'/>
                                    </Border>
                                </ControlTemplate>
                            </Button.Template>
                        </Button>

                        <Button Name='btnModalClose' Grid.Column='2' Width='60' Height='38' Cursor='Hand'>
                            <Button.Template>
                                <ControlTemplate TargetType='Button'>
                                    <Border Background='#1E293B' CornerRadius='6'>
                                        <TextBlock Text='Done' Foreground='#94A3B8' FontSize='12' HorizontalAlignment='Center' VerticalAlignment='Center'/>
                                    </Border>
                                </ControlTemplate>
                            </Button.Template>
                        </Button>
                    </Grid>
                </Grid>
            </Border>
        </Grid>
    </Grid>
</Border>
";
            var root = (Border)XamlReader.Parse(xaml);
            this.Content = root;
        }

        private void BindControls()
        {
            var root = (Border)this.Content;

            // Title bar border for dragging
            var titleBarBorder = (Border)root.FindName("titleBarBorder");
            if (titleBarBorder != null)
            {
                titleBarBorder.MouseLeftButtonDown += (s, e) =>
                {
                    if (e.ButtonState == MouseButtonState.Pressed)
                    {
                        this.DragMove();
                    }
                };
            }

            // Drag and drop folder support
            this.PreviewDragOver += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effects = DragDropEffects.Copy;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
                e.Handled = true;
            };

            this.Drop += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] items = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (items != null && items.Length > 0)
                    {
                        string path = items[0];
                        if (File.Exists(path))
                        {
                            path = Path.GetDirectoryName(path);
                        }
                        if (Directory.Exists(path))
                        {
                            _targetDirectory = path;
                            txtFolderPath.Text = _targetDirectory;
                            RefreshScan();
                        }
                    }
                }
            };

            // Controls
            txtFolderPath = (TextBox)root.FindName("txtFolderPath");
            txtFileCount = (TextBlock)root.FindName("txtFileCount");
            txtCategoryCount = (TextBlock)root.FindName("txtCategoryCount");
            txtStatusBadge = (TextBlock)root.FindName("txtStatusBadge");
            progressBar = (ProgressBar)root.FindName("progressBar");
            logParagraph = (Paragraph)root.FindName("logParagraph");
            logBox = (RichTextBox)root.FindName("logBox");

            btnOrganize = (Button)root.FindName("btnOrganize");
            btnBrowse = (Button)root.FindName("btnBrowse");
            btnOpenFolder = (Button)root.FindName("btnOpenFolder");
            btnRefresh = (Button)root.FindName("btnRefresh");
            btnClose = (Button)root.FindName("btnClose");
            btnMinimize = (Button)root.FindName("btnMinimize");
            btnThanksHeader = (Button)root.FindName("btnThanksHeader");
            btnThanksFooter = (Button)root.FindName("btnThanksFooter");

            chkMoveOthers = (CheckBox)root.FindName("chkMoveOthers");
            chkAutoOpen = (CheckBox)root.FindName("chkAutoOpen");

            overlayModal = (Grid)root.FindName("overlayModal");
            btnModalThanks = (Button)root.FindName("btnModalThanks");
            btnModalOpen = (Button)root.FindName("btnModalOpen");
            btnModalClose = (Button)root.FindName("btnModalClose");
            txtModalSummary = (TextBlock)root.FindName("txtModalSummary");

            // Event handlers
            btnClose.Click += (s, e) => this.Close();
            btnMinimize.Click += (s, e) => this.WindowState = WindowState.Minimized;

            btnThanksHeader.Click += (s, e) => OpenWebsite();
            btnThanksFooter.Click += (s, e) => OpenWebsite();

            btnBrowse.Click += (s, e) => BrowseFolder();
            btnOpenFolder.Click += (s, e) => OpenFolderInExplorer();
            btnRefresh.Click += (s, e) => RefreshScan();

            btnOrganize.Click += async (s, e) => await StartOrganizationAsync();

            btnModalThanks.Click += (s, e) =>
            {
                OpenWebsite();
                overlayModal.Visibility = Visibility.Collapsed;
            };
            btnModalOpen.Click += (s, e) =>
            {
                OpenFolderInExplorer();
                overlayModal.Visibility = Visibility.Collapsed;
            };
            btnModalClose.Click += (s, e) =>
            {
                overlayModal.Visibility = Visibility.Collapsed;
            };

            txtFolderPath.Text = _targetDirectory;
        }

        private void OpenWebsite()
        {
            try
            {
                Process.Start("https://www.basharbillah.com");
            }
            catch (Exception ex)
            {
                AppendLog("Error opening website: " + ex.Message, Brushes.Tomato);
            }
        }

        private void OpenFolderInExplorer()
        {
            try
            {
                if (Directory.Exists(_targetDirectory))
                {
                    Process.Start("explorer.exe", _targetDirectory);
                }
            }
            catch (Exception ex)
            {
                AppendLog("Error opening explorer: " + ex.Message, Brushes.Tomato);
            }
        }

        private void BrowseFolder()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.Description = "Select a folder to organize";
                dialog.SelectedPath = _targetDirectory;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _targetDirectory = dialog.SelectedPath;
                    txtFolderPath.Text = _targetDirectory;
                    RefreshScan();
                }
            }
        }

        private void RefreshScan()
        {
            try
            {
                if (!Directory.Exists(_targetDirectory))
                {
                    AppendLog("[ERROR] Target directory does not exist: " + _targetDirectory, Brushes.Tomato);
                    return;
                }

                string currentExeName = Process.GetCurrentProcess().MainModule.ModuleName;
                string[] files = Directory.GetFiles(_targetDirectory);

                int looseCount = 0;
                var matchedCategories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                for (int i = 0; i < files.Length; i++)
                {
                    string f = files[i];
                    string filename = Path.GetFileName(f);
                    string ext = Path.GetExtension(f).TrimStart('.');

                    if (string.Equals(filename, currentExeName, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(filename, "Folder Organizer_ By Billah Studio.bat", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(filename, "Folder Organizer_ By Billah Studio.exe", StringComparison.OrdinalIgnoreCase) ||
                        _engine.TempExtensions.Contains(ext))
                    {
                        continue;
                    }

                    looseCount++;
                    string cat = _engine.FindCategory(ext);
                    if (!string.IsNullOrEmpty(cat))
                    {
                        matchedCategories.Add(cat);
                    }
                    else
                    {
                        matchedCategories.Add("Others");
                    }
                }

                txtFileCount.Text = looseCount.ToString();
                txtCategoryCount.Text = matchedCategories.Count.ToString();
                txtStatusBadge.Text = looseCount > 0 ? "Ready to Organize" : "No Loose Files";
                txtStatusBadge.Foreground = looseCount > 0 ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34D399")) : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));

                logParagraph.Inlines.Clear();
                AppendLog(string.Format("[READY] Selected: {0}", _targetDirectory), Brushes.SkyBlue);
                AppendLog(string.Format("[SCAN] Found {0} loose file(s) across {1} category folder(s).", looseCount, matchedCategories.Count), Brushes.LightGreen);
            }
            catch (Exception ex)
            {
                AppendLog("[ERROR] Failed to scan: " + ex.Message, Brushes.Tomato);
            }
        }

        private async Task StartOrganizationAsync()
        {
            if (!Directory.Exists(_targetDirectory))
            {
                System.Windows.MessageBox.Show("Selected directory does not exist!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            btnOrganize.IsEnabled = false;
            btnBrowse.IsEnabled = false;
            btnRefresh.IsEnabled = false;
            txtStatusBadge.Text = "Organizing Files...";
            txtStatusBadge.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FBBF24"));
            progressBar.Value = 0;

            bool moveOthers = chkMoveOthers.IsChecked == true;
            bool autoOpen = chkAutoOpen.IsChecked == true;
            string currentExeName = Process.GetCurrentProcess().MainModule.ModuleName;

            int movedCount = 0;
            int skippedCount = 0;

            await Task.Run(new Action(() =>
            {
                try
                {
                    string[] files = Directory.GetFiles(_targetDirectory);
                    int total = files.Length;

                    for (int i = 0; i < total; i++)
                    {
                        string filePath = files[i];
                        string fileName = Path.GetFileName(filePath);
                        string ext = Path.GetExtension(filePath).TrimStart('.');

                        if (string.Equals(fileName, currentExeName, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(fileName, "Folder Organizer_ By Billah Studio.bat", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(fileName, "Folder Organizer_ By Billah Studio.exe", StringComparison.OrdinalIgnoreCase) ||
                            _engine.TempExtensions.Contains(ext))
                        {
                            skippedCount++;
                            int progress = (int)(((double)(i + 1) / total) * 100);
                            Dispatcher.Invoke(new Action(() =>
                            {
                                progressBar.Value = progress;
                                AppendLog(string.Format("[SKIP] {0} (Ignored)", fileName), Brushes.Gray);
                            }));
                            continue;
                        }

                        string targetSubfolder = _engine.FindCategory(ext);
                        if (string.IsNullOrEmpty(targetSubfolder))
                        {
                            if (moveOthers)
                            {
                                targetSubfolder = "Others";
                            }
                            else
                            {
                                skippedCount++;
                                int progress = (int)(((double)(i + 1) / total) * 100);
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    progressBar.Value = progress;
                                    AppendLog(string.Format("[SKIP] {0} (Uncategorized)", fileName), Brushes.DarkGray);
                                }));
                                continue;
                            }
                        }

                        string destDir = Path.Combine(_targetDirectory, targetSubfolder);
                        if (!Directory.Exists(destDir))
                        {
                            Directory.CreateDirectory(destDir);
                        }

                        string destFile = Path.Combine(destDir, fileName);
                        destFile = _engine.GetSafeDestinationPath(destFile);

                        try
                        {
                            File.Move(filePath, destFile);
                            movedCount++;

                            int progress = (int)(((double)(i + 1) / total) * 100);
                            string logFolder = targetSubfolder;
                            string destName = Path.GetFileName(destFile);

                            Dispatcher.Invoke(new Action(() =>
                            {
                                progressBar.Value = progress;
                                AppendLog(string.Format("[MOVED] {0}  ➔  {1}\\", destName, logFolder), Brushes.LightGreen);
                            }));
                        }
                        catch (Exception moveEx)
                        {
                            skippedCount++;
                            Dispatcher.Invoke(new Action(() =>
                            {
                                AppendLog(string.Format("[FAIL] {0}: {1}", fileName, moveEx.Message), Brushes.Tomato);
                            }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        AppendLog("[CRITICAL ERROR] " + ex.Message, Brushes.Red);
                    }));
                }
            }));

            progressBar.Value = 100;
            txtStatusBadge.Text = "Finished!";
            txtStatusBadge.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34D399"));
            btnOrganize.IsEnabled = true;
            btnBrowse.IsEnabled = true;
            btnRefresh.IsEnabled = true;

            AppendLog(string.Format("[COMPLETED] Successfully organized {0} file(s). Skipped: {1}", movedCount, skippedCount), Brushes.DeepSkyBlue);

            RefreshScan();

            txtModalSummary.Text = string.Format("Successfully sorted {0} file(s) into their respective folders.", movedCount);
            overlayModal.Visibility = Visibility.Visible;

            if (autoOpen)
            {
                OpenFolderInExplorer();
            }
        }

        private void AppendLog(string message, Brush brush)
        {
            var run = new Run(string.Format("[{0}] {1}\n", DateTime.Now.ToString("HH:mm:ss"), message))
            {
                Foreground = brush
            };
            logParagraph.Inlines.Add(run);
            logBox.ScrollToEnd();
        }
    }
}
