using System;

namespace CnnConvolutionSimulator
{
    /// <summary>
    /// Konvolüsyon işlemi için kullanılan matematiksel çekirdeği (filtreyi) kapsüller.
    /// Bu sınıf, veriyi (matris) işleme mantığından ayırarak OOP prensiplerini takip eder.
    /// </summary>
    public class MatrixKernel
    {
        // Filtreyi temsil eden 2D dizi.
        // 3x3'lük bir çekirdek için bu double[3,3] olacaktır.
        public double[,] Kernel { get; private set; }

        public int Width => Kernel.GetLength(1);
        public int Height => Kernel.GetLength(0);

        public MatrixKernel(double[,] kernel)
        {
            Kernel = kernel;
        }

        // --- Yaygın Filtreler için Fabrika Metotları ---

        /// <summary>
        /// Standart bir Kenar Algılama (Edge Detection) çekirdeği döndürür.
        /// Yüksek kontrastlı alanları (kenarları) vurgular.
        /// </summary>
        public static MatrixKernel EdgeDetection()
        {
            // Yaygın bir kenar algılama çekirdeği (Laplacian operatörü yaklaşımı)
            //  0 -1  0
            // -1  4 -1
            //  0 -1  0
            return new MatrixKernel(new double[,] {
                {  0, -1,  0 },
                { -1,  4, -1 },
                {  0, -1,  0 }
            });
        }

        /// <summary>
        /// Keskinleştirme (Sharpen) çekirdeği döndürür.
        /// Bitişik pikseller arasındaki farkları artırır.
        /// </summary>
        public static MatrixKernel Sharpen()
        {
            //  0 -1  0
            // -1  5 -1
            //  0 -1  0
            return new MatrixKernel(new double[,] {
                {  0, -1,  0 },
                { -1,  5, -1 },
                {  0, -1,  0 }
            });
        }

        /// <summary>
        /// Gaussian Bulanıklaştırma (Blur) çekirdeği döndürür (3x3 yaklaşımı).
        /// Komşu piksellerin ağırlıklı ortalamasını alarak görüntüyü yumuşatır.
        /// </summary>
        public static MatrixKernel GaussianBlur()
        {
            // Gaussian Blur 3x3
            // 1/16  2/16  1/16
            // 2/16  4/16  2/16
            // 1/16  2/16  1/16
            
            // Tamsayı olarak tanımlayıp 16.0'a bölebiliriz
            return new MatrixKernel(new double[,] {
                { 1.0/16, 2.0/16, 1.0/16 },
                { 2.0/16, 4.0/16, 2.0/16 },
                { 1.0/16, 2.0/16, 1.0/16 }
            });
        }

        /// <summary>
        /// Kutu Bulanıklaştırma (Box Blur) çekirdeği döndürür.
        /// Komşuların basit ortalamasıdır.
        /// </summary>
        public static MatrixKernel BoxBlur()
        {
            // 1/9 1/9 1/9
            // 1/9 1/9 1/9
            // 1/9 1/9 1/9
            return new MatrixKernel(new double[,] {
                { 1.0/9, 1.0/9, 1.0/9 },
                { 1.0/9, 1.0/9, 1.0/9 },
                { 1.0/9, 1.0/9, 1.0/9 }
            });
        }
        /// <summary>
        /// Kabartma (Emboss) çekirdeği döndürür.
        /// Görüntüye 3D derinlik hissi verir.
        /// </summary>
        public static MatrixKernel Emboss()
        {
            //  -2 -1  0
            //  -1  1  1
            //   0  1  2
            return new MatrixKernel(new double[,] {
                { -2, -1,  0 },
                { -1,  1,  1 },
                {  0,  1,  2 }
            });
        }

        /// <summary>
        /// Sobel Yatay Kenar Algılama çekirdeği.
        /// Yatay kenarları vurgular.
        /// </summary>
        public static MatrixKernel SobelHorizontal()
        {
            // -1 -2 -1
            //  0  0  0
            //  1  2  1
            return new MatrixKernel(new double[,] {
                { -1, -2, -1 },
                {  0,  0,  0 },
                {  1,  2,  1 }
            });
        }

        /// <summary>
        /// Sobel Dikey Kenar Algılama çekirdeği.
        /// Dikey kenarları vurgular.
        /// </summary>
        public static MatrixKernel SobelVertical()
        {
            // -1  0  1
            // -2  0  2
            // -1  0  1
            return new MatrixKernel(new double[,] {
                { -1,  0,  1 },
                { -2,  0,  2 },
                { -1,  0,  1 }
            });
        }
    }
}
