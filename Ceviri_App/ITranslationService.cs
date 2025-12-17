using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceviri_App
{
    // SOYUTLAMA (ABSTRACTION) PRENSİBİ:
    // Çeviri işleminin "nasıl" yapıldığını gizlemek ve sadece "ne" yapıldığını belirtmek için Interface kullanıyoruz.
    // Bu sayede, ileride Google Translate, Yandex Translate veya Mock (Test) servisi gibi farklı implementasyonları
    // kodumuzu değiştirmeden kullanabiliriz (Open/Closed Principle).
    public interface ITranslationService
    {
        /// <summary>
        /// Verilen metni kaynak dilden hedef dile çevirir.
        /// </summary>
        /// <param name="text">Çevrilecek metin</param>
        /// <param name="fromLang">Kaynak dil kodu (örn: "en")</param>
        /// <param name="toLang">Hedef dil kodu (örn: "tr")</param>
        /// <returns>Çevrilmiş metin</returns>
        string Translate(string text, string fromLang, string toLang);
    }
}
