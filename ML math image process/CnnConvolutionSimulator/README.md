# CNN Konvolüsyon Simülatörü (CNN Convolution Simulator)

Bu proje, Konvolüsyonel Sinir Ağları'nın (CNN) temel yapı taşı olan **Konvolüsyon (Convolution)** işlemini görselleştirmek ve simüle etmek amacıyla geliştirilmiş bir C# Windows Forms uygulamasıdır.

Makine Öğrenmesi ve Görüntü İşleme temellerini anlamak isteyenler için eğitici bir araçtır.

## 🚀 Özellikler

*   **Çeşitli Filtreler**: Hazır filtreler ile görüntü işleme:
    *   Kenar Algılama (Edge Detection)
    *   Keskinleştirme (Sharpen)
    *   Gaussian Bulanıklaştırma (Blur)
    *   Kutu Bulanıklaştırma (Box Blur)
    *   Kabartma (Emboss)
    *   Sobel Operatörleri (Yatay ve Dikey)
*   **Özel Filtre (Custom Kernel)**: Kendi 3x3 matris değerlerinizi girerek özel filtreler oluşturabilir ve deneyebilirsiniz.
*   **Yüksek Performans**: `LockBits` ve `unsafe` kod blokları kullanılarak optimize edilmiştir. Büyük görselleri milisaniyeler içinde işler.
*   **Modern Arayüz**: Göz yormayan koyu tema (Dark Mode) ve duyarlı (responsive) tasarım.
*   **Kayıt Özelliği**: İşlenen görüntüleri PNG, JPG veya BMP formatında kaydedebilirsiniz.

## 🛠️ Teknolojiler

*   **Dil**: C#
*   **Framework**: .NET 8.0
*   **Arayüz**: Windows Forms (WinForms)
*   **Mimari**: OOP (Nesne Yönelimli Programlama) prensiplerine uygun katmanlı yapı.

## 💻 Kurulum ve Çalıştırma

1.  Projeyi klonlayın veya indirin.
2.  Terminali açın ve proje dizinine gidin:
    ```bash
    cd CnnConvolutionSimulator
    ```
3.  Uygulamayı derleyin ve çalıştırın:
    ```bash
    dotnet run
    ```

## 🧠 Nasıl Çalışır?

Uygulama, yüklenen görüntünün her bir pikseli üzerinde 3x3'lük bir matris (kernel) gezdirir. Her adımda:
1.  Piksel ve komşuları, kernel değerleri ile çarpılır.
2.  Sonuçlar toplanır (Multiply and Accumulate).
3.  Elde edilen değer 0-255 aralığına sıkıştırılır (Clamping).
4.  Yeni piksel değeri sonuç görüntüsüne yazılır.

Bu işlem, CNN'lerin görüntülerden öznitelik (feature) çıkarmak için kullandığı yöntemin aynısıdır.

## 📂 Proje Yapısı

*   `MatrixKernel.cs`: Matematiksel filtre matrislerini tanımlayan sınıf.
*   `ConvolutionEngine.cs`: Görüntü işleme ve konvolüsyon algoritmasının çalıştığı motor.
*   `MainForm.cs`: Kullanıcı arayüzü ve olay yönetimi.

## 📝 Lisans

Bu proje eğitim amaçlı açık kaynak kodludur. İstediğiniz gibi kullanabilir ve geliştirebilirsiniz.
