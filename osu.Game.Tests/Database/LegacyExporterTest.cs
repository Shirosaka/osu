﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using NUnit.Framework;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Platform;
using osu.Framework.Testing;
using osu.Game.Database;

namespace osu.Game.Tests.Database
{
    public class LegacyExporterTest
    {
        private TestLegacyExporter legacyExporter = null!;
        private TemporaryNativeStorage storage = null!;

        [SetUp]
        public void SetUp()
        {
            storage = new TemporaryNativeStorage("export-storage");
            legacyExporter = new TestLegacyExporter(storage);
        }

        [Test]
        public void ExportFileWithNormalNameTest()
        {
            var exportStorage = storage.GetStorageForDirectory(@"exports");

            const string filename = "normal file name";
            var item = new TestPathInfo(filename);

            Assert.That(item.Filename.Length < TestLegacyExporter.GetMaxPath(), Is.True);
            exportItemAndAssert(item, exportStorage, filename);
        }

        [Test]
        public void ExportFileWithNormalNameMultipleTimesTest()
        {
            var exportStorage = storage.GetStorageForDirectory(@"exports");

            const string filename = "normal file name";
            var item = new TestPathInfo(filename);

            Assert.That(item.Filename.Length < TestLegacyExporter.GetMaxPath(), Is.True);

            //Export multiple times
            for (int i = 0; i < 10; i++)
            {
                string expectedFileName = i == 0 ? filename : $"{filename} ({i})";
                exportItemAndAssert(item, exportStorage, expectedFileName);
            }
        }

        [Test]
        public void ExportFileWithSuperLongNameTest()
        {
            var exportStorage = storage.GetStorageForDirectory(@"exports");

            const string fullname =
                "some file with super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name";

            int expectedLength = TestLegacyExporter.GetMaxPath() - (legacyExporter.GetExtension().Length);
            string expectedName = fullname.Remove(expectedLength);

            var item = new TestPathInfo(fullname);

            Assert.That(item.Filename.Length > TestLegacyExporter.GetMaxPath(), Is.True);
            exportItemAndAssert(item, exportStorage, expectedName);
        }

        [Test]
        public void ExportFileWithSuperLongNameMultipleTimesTest()
        {
            var exportStorage = storage.GetStorageForDirectory(@"exports");

            const string fullname =
                "some file with super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name super long name";

            int expectedLength = TestLegacyExporter.GetMaxPath() - (legacyExporter.GetExtension().Length);
            string expectedName = fullname.Remove(expectedLength);

            var item = new TestPathInfo(fullname);

            Assert.That(item.Filename.Length > TestLegacyExporter.GetMaxPath(), Is.True);

            //Export multiple times
            for (int i = 0; i < 10; i++)
                exportItemAndAssert(item, exportStorage, expectedName);
        }

        private void exportItemAndAssert(IHasNamedFiles item, Storage exportStorage, string expectedName)
        {
            Assert.DoesNotThrow(() => legacyExporter.Export(item));
            Assert.That(exportStorage.Exists($"{expectedName}{legacyExporter.GetExtension()}"), Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            if (storage.IsNotNull())
                storage.Dispose();
        }

        private class TestPathInfo : IHasNamedFiles
        {
            public string Filename { get; }

            public IEnumerable<INamedFileUsage> Files { get; } = new List<INamedFileUsage>();

            public TestPathInfo(string filename)
            {
                Filename = filename;
            }

            public override string ToString() => Filename;
        }

        private class TestLegacyExporter : LegacyExporter<IHasNamedFiles>
        {
            public TestLegacyExporter(Storage storage)
                : base(storage)
            {
            }

            public static int GetMaxPath() => MAX_PATH;

            public string GetExtension() => FileExtension;

            protected override string FileExtension => ".ots";
        }
    }
}
