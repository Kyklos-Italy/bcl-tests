using System;
using Xunit;
using Kyklos.Kernel.Graphics;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using FluentAssertions;
using Kyklos.Kernel.Graphics.Test.Support;
using Kyklos.Kernel.Graphics.Test.Support.Mock;
using static Kyklos.Kernel.Graphics.Test.Support.Framework;
namespace Kyklos.Kernel.Graphics.Test.Graphics
{
    public class GraphicsBaseTestMethods
    {
        private MockData<PngMockData> MockDataPng { get; }
        private MockData<JpgMockData> MockDataJpg { get; }
        private MockData<GifMockData> MockDataGif { get; }
        private MockData<BmpMockData> MockDataBmp { get; }
        private MockData<TifMockData> MockDataTif { get; }

        public GraphicsBaseTestMethods(FrameworkType frameworkType)
        {
            MockDataPng = new MockData<PngMockData>(frameworkType);
            MockDataJpg = new MockData<JpgMockData>(frameworkType);
            MockDataGif = new MockData<GifMockData>(frameworkType);
            MockDataBmp = new MockData<BmpMockData>(frameworkType);
            MockDataTif = new MockData<TifMockData>(frameworkType);
        }

        public class PngMockData { }

        public class JpgMockData { }

        public class GifMockData { }

        public class BmpMockData { }

        public class TifMockData { }

        #region PNG

        protected void PngMustBeAnImageCore()
        {
            Assert.True(ImageUtility.IsImage(MockDataPng.Bytes));
        }
        
        protected void PngHasToBeConvertedFromByteArrayCore()
        {
            Image expectedImage = MockDataPng.Image;
            Image actualImage = ImageUtility.ConvertByteArrayToImage(MockDataPng.Bytes);
            actualImage.Should().BeEquivalentTo(actualImage);
        }

        protected void PngMustBeConvertedToByteArrayCore()
        {
            byte[] actualBytes = ImageUtility.ConvertImageToByteArray(MockDataPng.Image, MockDataPng.ImageFormat);
            Image actualImageFromBytes = Utils.ImageFromByteArray(actualBytes);
            Assert.True(Utils.IsValidImage(actualImageFromBytes));
        }
   
        protected void PngMustBeConvertedToStreamCore()
        {
            Stream actualStream = ImageUtility.ImageToStream(MockDataPng.Image);
            Image actualImageFromStream = Image.FromStream(actualStream);
            Assert.True(Utils.IsValidImage(actualImageFromStream));
        }

        protected void PngMustBeResizedToHalfCore()
        {
            Size expectedSize = new Size((int)Math.Ceiling((double)MockDataPng.Image.Width / 2), (int)Math.Ceiling((double)MockDataPng.Image.Height / 2));
            Image imageResized = ImageUtility.ResizeImage(MockDataPng.Image, -50);
            Size actualSize = new Size(imageResized.Width, imageResized.Height);
            //SaveImageToPath(mockImage.Image, mockImage.ImageFormat, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Images/Resized/" + Path.GetFileName(mockImage.Path));
            //imageResized.Save(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Images/Resized/" + Path.GetFileName(mockImage.Path), mockImage.ImageFormat);
            //SaveImageToPath(imageResized, mockImage.ImageFormat, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Images/Resized/" + Path.GetFileName(mockImage.Path));
            expectedSize.Should().BeEquivalentTo(actualSize);
        }

        protected void PngMustBeSavedFromByteArrayCore()
        {
            Image expectedImage = MockDataPng.Image;
            Image actualImage = ImageUtility.SafeByteArrayToImage(MockDataPng.Bytes);
            expectedImage.Should().BeEquivalentTo(expectedImage);
        }
        
        //protected void PngImageStreamMustBeResizedToHalf()
        //{
        //    MockImage<PngMockImage> mockImage = new MockImage<PngMockImage>(_framework);

        //    Stream resizedStream = ImageUtility.ResizeImageStream(
        //        ImageHelper.ImageToStream(mockImage.Image, mockImage.ImageFormat),
        //        ImageHelper.GetReducedImageSizeToHalf(mockImage.Image),
        //        null
        //    );
        //}

        #endregion

        #region JPG

        protected void JpgMustBeAnImageCore()
        {
            Assert.True(ImageUtility.IsImage(MockDataJpg.Bytes));
        }
        
        protected void JpgHasToBeConvertedFromByteArrayCore()
        {
            Image expectedImage = MockDataJpg.Image;
            Image actualImage = ImageUtility.ConvertByteArrayToImage(MockDataJpg.Bytes);
            actualImage.Should().BeEquivalentTo(actualImage);
        }
        
        protected void JpgMustBeConvertedToByteArrayCore()
        {
            byte[] actualBytes = ImageUtility.ConvertImageToByteArray(MockDataJpg.Image, MockDataJpg.ImageFormat);
            Image actualImageFromBytes = Utils.ImageFromByteArray(actualBytes);
            Assert.True(Utils.IsValidImage(actualImageFromBytes));
        }

        protected void JpgMustBeConvertedToStreamCore()
        {
            Stream actualStream = ImageUtility.ImageToStream(MockDataJpg.Image);
            Image actualImageFromStream = Image.FromStream(actualStream);
            Assert.True(Utils.IsValidImage(actualImageFromStream));
        }
        
        protected void JpgMustBeResizedToHalfCore()
        {
            Size expectedSize = new Size((int)Math.Ceiling((double)MockDataJpg.Image.Width / 2), (int)Math.Ceiling((double)MockDataJpg.Image.Height / 2));
            Image imageResized = ImageUtility.ResizeImage(MockDataJpg.Image, -50);
            Size actualSize = new Size(imageResized.Width, imageResized.Height);
            //SaveImageToPath(mockImage.Image, mockImage.ImageFormat, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Images/Resized/" + Path.GetFileName(mockImage.Path));
            //imageResized.Save(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Images/Resized/" + Path.GetFileName(mockImage.Path), mockImage.ImageFormat);
            //SaveImageToPath(imageResized, mockImage.ImageFormat, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Images/Resized/" + Path.GetFileName(mockImage.Path));
            expectedSize.Should().BeEquivalentTo(actualSize);
        }

        protected void JpgMustBeSavedFromByteArrayCore()
        {
            Image expectedImage = MockDataJpg.Image;
            Image actualImage = ImageUtility.SafeByteArrayToImage(MockDataJpg.Bytes);
            expectedImage.Should().BeEquivalentTo(expectedImage);
        }

        #endregion

        #region GIF

        protected void GifMustBeAnImageCore()
        {
            Assert.True(ImageUtility.IsImage(MockDataGif.Bytes));
        }

        protected void GifHasToBeConvertedFromByteArrayCore()
        {
            Image expectedImage = MockDataGif.Image;
            Image actualImage = ImageUtility.ConvertByteArrayToImage(MockDataGif.Bytes);
            actualImage.Should().BeEquivalentTo(actualImage);
        }

        protected void GifMustBeConvertedToByteArrayCore()
        {
            byte[] actualBytes = ImageUtility.ConvertImageToByteArray(MockDataGif.Image, MockDataGif.ImageFormat);
            Image actualImageFromBytes = Utils.ImageFromByteArray(actualBytes);
            Assert.True(Utils.IsValidImage(actualImageFromBytes));
        }

        protected void GifMustBeConvertedToStreamCore()
        {
            Stream actualStream = ImageUtility.ImageToStream(MockDataGif.Image);
            Image actualImageFromStream = Image.FromStream(actualStream);
            Assert.True(Utils.IsValidImage(actualImageFromStream));
        }

        protected void GifMustBeResizedToHalfCore()
        {
            Size expectedSize = new Size((int)Math.Ceiling((double)MockDataGif.Image.Width / 2), (int)Math.Ceiling((double)MockDataGif.Image.Height / 2));
            Image imageResized = ImageUtility.ResizeImage(MockDataGif.Image, -50);
            Size actualSize = new Size(imageResized.Width, imageResized.Height);
            //SaveImageToPath(mockImage.Image, mockImage.ImageFormat, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Images/Resized/" + Path.GetFileName(mockImage.Path));
            //imageResized.Save(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Images/Resized/" + Path.GetFileName(mockImage.Path), mockImage.ImageFormat);
            //SaveImageToPath(imageResized, mockImage.ImageFormat, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Images/Resized/" + Path.GetFileName(mockImage.Path));
            expectedSize.Should().BeEquivalentTo(actualSize);
        }

        protected void GifMustBeSavedFromByteArrayCore()
        {
            Image expectedImage = MockDataGif.Image;
            Image actualImage = ImageUtility.SafeByteArrayToImage(MockDataGif.Bytes);
            expectedImage.Should().BeEquivalentTo(expectedImage);
        }

        #endregion

        #region BMP

        protected void BmpMustBeAnImageCore()
        {
            Assert.True(ImageUtility.IsImage(MockDataBmp.Bytes));
        }

        protected void BmpHasToBeConvertedFromByteArrayCore()
        {
            Image expectedImage = MockDataBmp.Image;
            Image actualImage = ImageUtility.ConvertByteArrayToImage(MockDataBmp.Bytes);
            actualImage.Should().BeEquivalentTo(actualImage);
        }
        
        protected void BmpMustBeConvertedToByteArrayCore()
        {
            byte[] actualBytes = ImageUtility.ConvertImageToByteArray(MockDataBmp.Image, MockDataBmp.ImageFormat);
            Image actualImageFromBytes = Utils.ImageFromByteArray(actualBytes);
            Assert.True(Utils.IsValidImage(actualImageFromBytes));
        }
        
        protected void BmpMustBeConvertedToStreamCore()
        {
            Stream actualStream = ImageUtility.ImageToStream(MockDataBmp.Image);
            Image actualImageFromStream = Image.FromStream(actualStream);
            Assert.True(Utils.IsValidImage(actualImageFromStream));
        }
        
        protected void BmpMustBeResizedToHalfCore()
        {
            Size expectedSize = new Size((int)Math.Ceiling((double)MockDataBmp.Image.Width / 2), (int)Math.Ceiling((double)MockDataBmp.Image.Height / 2));
            Image imageResized = ImageUtility.ResizeImage(MockDataBmp.Image, -50);
            Size actualSize = new Size(imageResized.Width, imageResized.Height);
            //SaveImageToPath(mockImage.Image, mockImage.ImageFormat, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Images/Resized/" + Path.GetFileName(mockImage.Path));
            //imageResized.Save(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Images/Resized/" + Path.GetFileName(mockImage.Path), mockImage.ImageFormat);
            //SaveImageToPath(imageResized, mockImage.ImageFormat, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Images/Resized/" + Path.GetFileName(mockImage.Path));
            expectedSize.Should().BeEquivalentTo(actualSize);
        }

        protected void BmpMustBeSavedFromByteArrayCore()
        {
            Image expectedImage = MockDataBmp.Image;
            Image actualImage = ImageUtility.SafeByteArrayToImage(MockDataBmp.Bytes);
            expectedImage.Should().BeEquivalentTo(expectedImage);
        }

        #endregion

        #region TIF
        
        protected void TifMustBeAnImageCore()
        {
            Assert.True(ImageUtility.IsImage(MockDataTif.Bytes));
        }

        protected void TifHasToBeConvertedFromByteArrayCore()
        {
            Image expectedImage = MockDataTif.Image;
            Image actualImage = ImageUtility.ConvertByteArrayToImage(MockDataTif.Bytes);
            actualImage.Should().BeEquivalentTo(actualImage);
        }
        
        protected void TifMustBeConvertedToByteArrayCore()
        {
            byte[] actualBytes = ImageUtility.ConvertImageToByteArray(MockDataTif.Image, MockDataTif.ImageFormat);
            Image actualImageFromBytes = Utils.ImageFromByteArray(actualBytes);
            Assert.True(Utils.IsValidImage(actualImageFromBytes));
        }

        protected void TifMustBeConvertedToStreamCore()
        {
            Stream actualStream = ImageUtility.ImageToStream(MockDataTif.Image);
            Image actualImageFromStream = Image.FromStream(actualStream);
            Assert.True(Utils.IsValidImage(actualImageFromStream));
        }

        protected void TifMustBeResizedToHalfCore()
        {
            Size expectedSize = new Size((int)Math.Ceiling((double)MockDataTif.Image.Width / 2), (int)Math.Ceiling((double)MockDataTif.Image.Height / 2));
            Image imageResized = ImageUtility.ResizeImage(MockDataTif.Image, -50);
            Size actualSize = new Size(imageResized.Width, imageResized.Height);
            //SaveImageToPath(mockImage.Image, mockImage.ImageFormat, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Images/Resized/" + Path.GetFileName(mockImage.Path));
            //imageResized.Save(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Images/Resized/" + Path.GetFileName(mockImage.Path), mockImage.ImageFormat);
            //SaveImageToPath(imageResized, mockImage.ImageFormat, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Images/Resized/" + Path.GetFileName(mockImage.Path));
            expectedSize.Should().BeEquivalentTo(actualSize);
        }

        protected void TifMustBeSavedFromByteArrayCore()
        {
            Image expectedImage = MockDataTif.Image;
            Image actualImage = ImageUtility.SafeByteArrayToImage(MockDataTif.Bytes);
            expectedImage.Should().BeEquivalentTo(expectedImage);
        }

        #endregion
    }
}