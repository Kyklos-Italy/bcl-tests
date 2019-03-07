using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Kyklos.Kernel.Graphics.Test.Support
{
    public static class Utils
    {
        public static bool IsValidImage(Image imageToCheck)
        {
            try
            {
                using (Image newImage = imageToCheck)
                { }
            }
            catch (OutOfMemoryException ex)
            {
                return false;
            }
            return true;
        }

        public static byte[] ImageToByteArray(string imagePath)
        {
            return File.ReadAllBytes(imagePath);
        }

        public static Image ImageFromByteArray(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return Image.FromStream(ms);
            }
        }

        public static Stream ImageToStream(Image image, ImageFormat format)
        {
            var stream = new System.IO.MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }

        public static Size GetReducedImageSizeToHalf(Image image)
        {
            Size sizeReducedToHalf = new Size((int)Math.Ceiling((double)image.Width / 2), (int)Math.Ceiling((double)image.Height / 2));
            return sizeReducedToHalf;
        }
    }
}
