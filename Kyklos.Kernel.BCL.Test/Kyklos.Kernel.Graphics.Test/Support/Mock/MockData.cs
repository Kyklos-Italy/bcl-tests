using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using static Kyklos.Kernel.Graphics.Test.Graphics.GraphicsBaseTestMethods;

namespace Kyklos.Kernel.Graphics.Test.Support.Mock
{
    internal class MockData<T> : Framework, IDisposable
    {
        public string ImagePath { get; }
        public Image Image { get; }
        public ImageFormat ImageFormat { get; }
        public byte[] Bytes { get; }

        public MockData(FrameworkType frameworkType) : base(frameworkType)
        {
            if (typeof(T) == typeof(PngMockData))
            {
                ImagePath = ResourceFolder + "/image360x240.png";
            }
            if (typeof(T) == typeof(JpgMockData))
            {
                ImagePath = ResourceFolder + "/image350x150.jpg";
            }
            if (typeof(T) == typeof(GifMockData))
            {
                ImagePath = ResourceFolder + "/gif.gif";
            }
            if (typeof(T) == typeof(BmpMockData))
            {
                ImagePath = ResourceFolder + "/bmp.bmp";
            }
            if (typeof(T) == typeof(TifMockData))
            {
                ImagePath = ResourceFolder + "/tiff.tif";
            }
            Image = Image.FromFile(ImagePath);
            ImageFormat = Image.RawFormat;
            Bytes = Utils.ImageToByteArray(ImagePath);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Image.Dispose();   
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
