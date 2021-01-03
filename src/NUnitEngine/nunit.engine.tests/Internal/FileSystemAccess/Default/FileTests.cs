﻿// ***********************************************************************
// Copyright (c) 2021 NUnit Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************
namespace NUnit.Engine.Tests.Internal.FileSystemAccess.Default
{
    using NUnit.Engine.Internal.FileSystemAccess.Default;
    using NUnit.Framework;
    using System.Linq;
    using System.Reflection;
    using SIO = System.IO;

    [TestFixture]
    public sealed class FileTests
    {
        private string GetTestFileLocation()
        {
#if NETCOREAPP1_1
            return Assembly.GetEntryAssembly().Location;
#else
            return Assembly.GetAssembly(typeof(FileTests)).Location;
#endif
        }

        [Test]
        public void Init()
        {
            var path = this.GetTestFileLocation();
            var parent = SIO.Path.GetDirectoryName(path);

            var file = new File(path);

            Assert.AreEqual(path, file.FullName);
            Assert.AreEqual(parent, file.Parent.FullName);
        }

        [Test]
        public void Init_PathIsNull()
        {
            Assert.That(() => new File(null), Throws.ArgumentNullException);
        }

        [Test]
        public void Init_InvalidPath_InvalidDirectory()
        {
            var path = SIO.Path.GetInvalidPathChars()[1] + this.GetTestFileLocation();

            Assert.That(() => new File(path), Throws.ArgumentException);
        }

        [Test]
        public void Init_InvalidPath_InvalidFileName()
        {
            char invalidCharThatIsNotInInvalidPathChars = SIO.Path.GetInvalidFileNameChars().Except(SIO.Path.GetInvalidPathChars()).FirstOrDefault();
            var path = this.GetTestFileLocation() + invalidCharThatIsNotInInvalidPathChars;

            Assert.That(() => new File(path), Throws.ArgumentException);
        }

        [Test]
        public void Init_EmptyPath()
        {
            Assert.That(() => new File(string.Empty), Throws.ArgumentException);
        }

        [Test]
        public void Init_NonExistingFile()
        {
            var path = this.GetTestFileLocation();
            while (SIO.File.Exists(path))
            {
                path += "a";
            }

            var parent = SIO.Path.GetDirectoryName(path);

            var file = new File(path);

            Assert.AreEqual(path, file.FullName);
            Assert.AreEqual(parent, file.Parent.FullName);
        }

        [Test]
        public void Init_PathIsDirectory()
        {
            var path = SIO.Directory.GetCurrentDirectory() + SIO.Path.DirectorySeparatorChar;

            Assert.That(() => new File(path), Throws.ArgumentException);
        }

    }
}
