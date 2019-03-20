using Kyklos.Kernel.Graphics.Test.Graphics;
using Xunit;
using static Kyklos.Kernel.Graphics.Test.Support.Framework;

namespace Kyklos.Kernel.Graphics.Test.NetFramework.Graphics
{
    public class GraphicsTestMethods : GraphicsBaseTestMethods
    {
        public GraphicsTestMethods() : base(FrameworkType.NETFRAMEWORK) { }

        #region PNG

        [Fact(DisplayName = "PngMustBeAnImage")]
        public void PngMustBeAnImage()
        {
            PngMustBeAnImageCore();
        }

        [Fact(DisplayName = "PngHasToBeConvertedFromByteArray")]
        public void PngHasToBeConvertedFromByteArray()
        {
            PngHasToBeConvertedFromByteArrayCore();
        }

        [Fact(DisplayName = "PngMustBeConvertedToByteArray")]
        public void PngMustBeConvertedToByteArray()
        {
            PngMustBeConvertedToByteArrayCore();
        }

        [Fact(DisplayName = "PngMustBeConvertedToStream")]
        public void PngMustBeConvertedToStream()
        {
            PngMustBeConvertedToStreamCore();
        }

        [Fact(DisplayName = "PngMustBeResizedToHalf")]
        public void PngMustBeResizedToHalf()
        {
            PngMustBeResizedToHalfCore();
        }

        [Fact(DisplayName = "PngMustBeSavedFromByteArray")]
        public void PngMustBeSavedFromByteArray()
        {
            PngMustBeSavedFromByteArrayCore();
        }

        //[Fact]
        //public void PngImageStreamMustBeResizedToHalf()
        //{
        //    MockImage<PngMockImage> mockImage = new MockImage<PngMockImage>();

        //    Stream resizedStream = ImageUtility.ResizeImageStream(
        //        ImageHelper.ImageToStream(mockImage.Image, mockImage.ImageFormat),
        //        ImageHelper.GetReducedImageSizeToHalf(mockImage.Image),
        //        null
        //    );
        //}

        #endregion

        #region JPG

        [Fact(DisplayName = "JpgMustBeAnImage")]
        public void JpgMustBeAnImage()
        {
            JpgMustBeAnImageCore();
        }

        [Fact(DisplayName = "JpgHasToBeConvertedFromByteArray")]
        public void JpgHasToBeConvertedFromByteArray()
        {
            JpgHasToBeConvertedFromByteArrayCore();
        }

        [Fact(DisplayName = "JpgMustBeConvertedToByteArray")]
        public void JpgMustBeConvertedToByteArray()
        {
            JpgMustBeConvertedToByteArrayCore();
        }

        [Fact(DisplayName = "JpgMustBeConvertedToStream")]
        public void JpgMustBeConvertedToStream()
        {
            JpgMustBeConvertedToStreamCore();
        }

        [Fact(DisplayName = "JpgMustBeResizedToHalf")]
        public void JpgMustBeResizedToHalf()
        {
            JpgMustBeResizedToHalfCore();
        }

        [Fact(DisplayName = "JpgMustBeSavedFromByteArray")]
        public void JpgMustBeSavedFromByteArray()
        {
            JpgMustBeSavedFromByteArrayCore();
        }

        #endregion

        #region GIF

        [Fact(DisplayName = "GifMustBeAnImage")]
        public void GifMustBeAnImage()
        {
            GifMustBeAnImageCore();
        }

        [Fact(DisplayName = "GifHasToBeConvertedFromByteArray")]
        public void GifHasToBeConvertedFromByteArray()
        {
            GifHasToBeConvertedFromByteArrayCore();
        }

        [Fact(DisplayName = "GifMustBeConvertedToByteArray")]
        public void GifMustBeConvertedToByteArray()
        {
            GifMustBeConvertedToByteArrayCore();
        }

        [Fact(DisplayName = "GifMustBeConvertedToStream")]
        public void GifMustBeConvertedToStream()
        {
            GifMustBeConvertedToStreamCore();
        }

        [Fact(DisplayName = "GifMustBeResizedToHalf")]
        public void GifMustBeResizedToHalf()
        {
            GifMustBeResizedToHalfCore();
        }

        [Fact(DisplayName = "GifMustBeSavedFromByteArray")]
        public void GifMustBeSavedFromByteArray()
        {
            GifMustBeSavedFromByteArrayCore();
        }

        #endregion

        #region BMP

        [Fact] //(DisplayName = "BmpMustBeAnImageDDDDD")]
        public void BmpMustBeAnImage()
        {
            BmpMustBeAnImageCore();
        }

        [Fact(DisplayName = "BmpHasToBeConvertedFromByteArray")]
        public void BmpHasToBeConvertedFromByteArray()
        {
            BmpHasToBeConvertedFromByteArrayCore();
        }

        [Fact(DisplayName = "BmpMustBeConvertedToByteArray")]
        public void BmpMustBeConvertedToByteArray()
        {
            BmpMustBeConvertedToByteArrayCore();
        }

        [Fact(DisplayName = "BmpMustBeConvertedToStream")]
        public void BmpMustBeConvertedToStream()
        {
            BmpMustBeConvertedToStreamCore();
        }

        [Fact(DisplayName = "BmpMustBeResizedToHalf")]
        public void BmpMustBeResizedToHalf()
        {
            BmpMustBeResizedToHalfCore();
        }

        [Fact(DisplayName = "BmpMustBeSavedFromByteArray")]
        public void BmpMustBeSavedFromByteArray()
        {
            BmpMustBeSavedFromByteArrayCore();
        }

        #endregion

        #region TIF

        [Fact(DisplayName = "TifMustBeAnImage")]
        public void TifMustBeAnImage()
        {
            TifMustBeAnImageCore();
        }

        [Fact(DisplayName = "TifHasToBeConvertedFromByteArray")]
        public void TifHasToBeConvertedFromByteArray()
        {
            TifHasToBeConvertedFromByteArrayCore();
        }

        [Fact(DisplayName = "TifMustBeConvertedToByteArray")]
        public void TifMustBeConvertedToByteArray()
        {
            TifMustBeConvertedToByteArrayCore();
        }

        [Fact(DisplayName = "TifMustBeConvertedToStream")]
        public void TifMustBeConvertedToStream()
        {
            TifMustBeConvertedToStreamCore();
        }

        [Fact(DisplayName = "TifMustBeResizedToHalf")]
        public void TifMustBeResizedToHalf()
        {
            TifMustBeResizedToHalfCore();
        }

        [Fact(DisplayName = "TifMustBeSavedFromByteArray")]
        public void TifMustBeSavedFromByteArray()
        {
            TifMustBeSavedFromByteArrayCore();
        }

        #endregion
    }
}
