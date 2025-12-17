using System;
using System.Windows.Forms;

namespace Ceviri_App
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // DEPENDENCY INJECTION (MANUEL DI):
            // Uygulamanın başlangıç noktasında (Composition Root), bağımlılıkları oluşturuyoruz.
            
            // 1. Adım: Hangi servisi kullanacağımıza karar veriyoruz.
            // Artık gerçek çeviri servisini (MyMemory API) kullanıyoruz.
            ITranslationService translationService = new OnlineTranslationService();

            // 2. Adım: Servisi Form'a enjekte ediyoruz.
            Form1 mainForm = new Form1(translationService);

            // 3. Adım: Formu başlatıyoruz.
            Application.Run(mainForm);
        }
    }
}