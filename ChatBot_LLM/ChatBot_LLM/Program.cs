using System;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.UserSkins;
using ChatBot_LLM.Infrastructure;
using ChatBot_LLM.Interfaces;
using ChatBot_LLM.Presenters;
using ChatBot_LLM.Services;
using ChatBot_LLM.Views;

namespace ChatBot_LLM
{
    internal static class Program
    {
        /// <summary>
        /// Uygulamanın ana giriş noktası
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Windows Forms başlatma
            ApplicationConfiguration.Initialize();

            // Premium UI ayarları - DirectX donanım hızlandırma
            InitializePremiumSettings();

            // DevExpress tema ayarı
            InitializeDevExpressSkin();

            // DI Container'ı yapılandır
            ConfigureServices();

            // Ana formu oluştur ve başlat
            RunApplication();
        }

        /// <summary>
        /// Premium UI özellikleri - DirectX ve High DPI
        /// </summary>
        private static void InitializePremiumSettings()
        {
            // DirectX donanım hızlandırma - yumuşak animasyonlar ve gölgeler
            DevExpress.XtraEditors.WindowsFormsSettings.ForceDirectXPaint();
            
            // High DPI desteği
            DevExpress.XtraEditors.WindowsFormsSettings.SetDPIAware();
            
            // Modern varsayılan font
            DevExpress.XtraEditors.WindowsFormsSettings.DefaultFont = new System.Drawing.Font("Segoe UI", 10f);
        }

        /// <summary>
        /// DevExpress temasını yapılandırır
        /// </summary>
        private static void InitializeDevExpressSkin()
        {
            // Varsayılan tema: Office 2019 Colorful (veya Bezier)
            UserLookAndFeel.Default.SetSkinStyle(SkinStyle.Office2019Colorful);
            
            // Alternatif temalar:
            // UserLookAndFeel.Default.SetSkinStyle(SkinStyle.Bezier);
            // UserLookAndFeel.Default.SetSkinStyle(SkinStyle.Office2019DarkGray);
            // UserLookAndFeel.Default.SetSkinStyle(SkinSvgPalette.Bezier.OfficeColorful);
        }

        /// <summary>
        /// Dependency Injection Container'ı yapılandırır
        /// Tüm servisleri kaydeder
        /// </summary>
        private static void ConfigureServices()
        {
            // Settings Service (Singleton)
            var settingsService = new SettingsService();
            ServiceContainer.RegisterSingleton<ISettingsService>(settingsService);

            // Message Repository (Singleton - aynı session için)
            var messageRepository = new InMemoryMessageRepository();
            ServiceContainer.RegisterSingleton<IMessageRepository>(messageRepository);

            // LLM Service (Singleton)
            var llmService = new OpenAIService(settingsService);
            ServiceContainer.RegisterSingleton<ILLMService>(llmService);
        }

        /// <summary>
        /// Uygulamayı başlatır
        /// MVP pattern ile form ve presenter'ı bağlar
        /// </summary>
        private static void RunApplication()
        {
            // MainForm oluştur (View)
            var mainForm = new MainForm();

            // Presenter oluştur ve View'e bağla (MVP)
            var presenter = new MainPresenter(
                mainForm,
                ServiceContainer.Resolve<ILLMService>(),
                ServiceContainer.Resolve<IMessageRepository>(),
                ServiceContainer.Resolve<ISettingsService>()
            );

            // Presenter'ı başlat
            presenter.Initialize();

            // Form kapandığında cleanup
            mainForm.FormClosed += (s, e) =>
            {
                presenter.Dispose();
                ServiceContainer.Clear();
            };

            // Uygulamayı çalıştır
            Application.Run(mainForm);
        }
    }
}