using System;
using System.IO;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;

namespace Antivirus.UI;

static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Enable visual styles
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetHighDpiMode(HighDpiMode.SystemAware);

        // Apply DevExpress theme - Dark modern look
        // Apply DevExpress theme - Modern Windows 11 Look
        UserLookAndFeel.Default.SetSkinStyle(SkinStyle.WXI);
        
        // Ensure data directories exist
        EnsureDirectoriesExist();

        // Run the main form
        Application.Run(new Forms.MainForm());
    }

    /// <summary>
    /// Ensures required data directories exist
    /// </summary>
    private static void EnsureDirectoriesExist()
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var dataPath = Path.Combine(basePath, "data");
        var quarantinePath = Path.Combine(dataPath, "Quarantine");

        if (!Directory.Exists(dataPath))
            Directory.CreateDirectory(dataPath);

        if (!Directory.Exists(quarantinePath))
            Directory.CreateDirectory(quarantinePath);
    }
}