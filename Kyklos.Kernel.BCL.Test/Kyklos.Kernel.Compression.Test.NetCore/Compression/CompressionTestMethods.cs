﻿using Kyklos.Kernel.Compression.Test.Compression;
using Kyklos.Kernel.Compression.Zip;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Kyklos.Kernel.Compression.Test.Support.Framework;

namespace Kyklos.Kernel.Compression.Test.NetCore.Compression
{
    public class CompressionTestMethods : CompressionBaseTestMethods
    {
        public CompressionTestMethods() : base(FrameworkType.NETCORE) { }

        [Fact(DisplayName = "CreateZipFileFromFileList")]
        public void CreateZipFileFromFileList()
        {
            CreateZipFileFromFileListCore();
        }

        [Fact(DisplayName = "CreateZipFileFromFileListWithFileNameOnly")]
        public void CreateZipFileFromFileListWithFileNameOnly()
        {
            CreateZipFileFromFileListWithFileNameOnlyCore();
        }

        [Fact(DisplayName = "ZipString")]
        public async Task ZipString()
        {
            await ZipStringCore().ConfigureAwait(false);
        }

        [Fact(DisplayName = "ZipByteArray")]
        public async Task ZipByteArray()
        {
            await ZipByteArrayCore().ConfigureAwait(false);
        }

        //[Fact(Skip = "da capire come fare", DisplayName = "ZipSingleFileContent")]
        //public async Task ZipSingleFileContent()
        //{
        //    await ZipSingleFileContentCore().ConfigureAwait(false);
        //}

        [Fact(DisplayName = "UnZipString")]
        public void UnZipString()
        {
            UnZipStringCore();
        }

        //[Fact(Skip = "da capire come fare", DisplayName = "WriteAllBytesAsync")]
        //public void WriteAllBytesAsync()
        //{
        //    WriteAllBytesAsyncCore();
        //}

        [Fact(DisplayName = "CreateZipFromFileContentList")]
        public async Task CreateZipFromFileContentList()
        {
            await CreateZipFromFileContentListCore().ConfigureAwait(false);
        }

        //[Fact(Skip = "da capire come fare", DisplayName = "UnZipSingleFileContentToStream")]
        //public async Task UnZipSingleFileContentToStream()
        //{
        //    await UnZipSingleFileContentToStreamCore().ConfigureAwait(false);
        //}

        [Fact]
        public async Task UnzipDllsFromNupkgFile()
        {
            await UnzipDllsFromNupkgFileCore();
        }
    }
}
