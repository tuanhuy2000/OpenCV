using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

namespace OpenCV.Common
{
    class Com
    {
        public Image<Bgr, byte> ByteToImage(byte[] a)
        {
            int width = 200;        // Image Width
            int height = 200;       // Image Height
            int stride = 200 * 3;   // Image Stide - Bytes per Row (3 bytes per pixel)

            // Create data for an Image 512x640 RGB - 983,040 Bytes
            byte[] sourceImgData = new byte[500 * 500];
            sourceImgData = a;

            // Pin the imgData in memory and create an IntPtr to it's location
            GCHandle pinnedArray = GCHandle.Alloc(sourceImgData, GCHandleType.Pinned);
            IntPtr pointer = pinnedArray.AddrOfPinnedObject();

            // Create an image from the imgData
            Image<Bgr, byte> img = new Image<Bgr, byte>(width, height, stride, pointer);

            // Free the memory
            pinnedArray.Free();

            return img;
        }

        public byte[] converterDemo(Image<Bgr, Byte> x)
        {
            // Convert the source Image to Bitmap
            Bitmap bitmap = x.ToBitmap();

            // Create a BitmapData object from the resulting Bitmap, locking the backing data in memory
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, 200, 200),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);

            // Create an output byte array
            byte[] destImgData = new byte[bitmapData.Stride * bitmap.Height];

            // Copy the byte array to the destination imgData
            Marshal.Copy(bitmapData.Scan0,
                destImgData,
                0,
                destImgData.Length);

            // Free the memory
            bitmap.UnlockBits(bitmapData);

            return destImgData;
        }
    }
}
