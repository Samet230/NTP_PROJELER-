using System;
using System.Drawing;

namespace CnnConvolutionSimulator
{
    /// <summary>
    /// Konvolüsyon işleminin temel matematiksel operasyonlarını yönetir.
    /// Bu sınıf, CNN'lerde bulunan "kayan pencere" (sliding window) işlemini simüle eder.
    /// </summary>
    public class ConvolutionEngine
    {
        /// <summary>
        /// Kaynak görüntüye bir konvolüsyon filtresi uygular.
        /// </summary>
        /// <param name="source">Orijinal görüntü.</param>
        /// <param name="kernel">Uygulanacak matematiksel filtre.</param>
        /// <returns>Filtre uygulanmış yeni bir Bitmap döndürür.</returns>
        /// <summary>
        /// Applies a convolution filter to the source image using LockBits for high performance.
        /// </summary>
        public Bitmap ApplyFilter(Bitmap source, MatrixKernel kernel)
        {
            // Create result bitmap
            Bitmap resultBitmap = new Bitmap(source.Width, source.Height);

            // Get kernel data
            int kernelWidth = kernel.Width;
            int kernelHeight = kernel.Height;
            double[,] filter = kernel.Kernel;
            int xOffset = kernelWidth / 2;
            int yOffset = kernelHeight / 2;

            // Lock source and result bits
            System.Drawing.Imaging.BitmapData srcData = source.LockBits(
                new Rectangle(0, 0, source.Width, source.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            System.Drawing.Imaging.BitmapData dstData = resultBitmap.LockBits(
                new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height),
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // Unsafe block for pointer arithmetic
            unsafe
            {
                byte* srcPtr = (byte*)srcData.Scan0;
                byte* dstPtr = (byte*)dstData.Scan0;
                int stride = srcData.Stride;
                int width = source.Width;
                int height = source.Height;

                // Loop through pixels (skipping borders)
                for (int y = yOffset; y < height - yOffset; y++)
                {
                    for (int x = xOffset; x < width - xOffset; x++)
                    {
                        double blueSum = 0;
                        double greenSum = 0;
                        double redSum = 0;

                        // Convolution loop
                        for (int ky = 0; ky < kernelHeight; ky++)
                        {
                            for (int kx = 0; kx < kernelWidth; kx++)
                            {
                                // Calculate neighbor position
                                int pixelX = x + kx - xOffset;
                                int pixelY = y + ky - yOffset;

                                // Get pointer to neighbor pixel
                                // Format24bppRgb: BGR order
                                byte* pixelPtr = srcPtr + (pixelY * stride) + (pixelX * 3);

                                double kernelVal = filter[ky, kx];

                                blueSum += pixelPtr[0] * kernelVal;
                                greenSum += pixelPtr[1] * kernelVal;
                                redSum += pixelPtr[2] * kernelVal;
                            }
                        }

                        // Clamp and set result
                        // Destination pointer
                        byte* dstPixelPtr = dstPtr + (y * stride) + (x * 3);
                        
                        dstPixelPtr[0] = (byte)Clamp((int)blueSum);
                        dstPixelPtr[1] = (byte)Clamp((int)greenSum);
                        dstPixelPtr[2] = (byte)Clamp((int)redSum);
                    }
                }
            }

            // Unlock bits
            source.UnlockBits(srcData);
            resultBitmap.UnlockBits(dstData);

            return resultBitmap;
        }

        /// <summary>
        /// Değerlerin 0-255 aralığında kalmasını sağlayan yardımcı metot.
        /// </summary>
        private int Clamp(int value)
        {
            if (value < 0) return 0;
            if (value > 255) return 255;
            return value;
        }
    }
}
