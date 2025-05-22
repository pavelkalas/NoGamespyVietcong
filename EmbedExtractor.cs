using System;
using System.IO;
using System.Reflection;
using System.Threading;

/**
 * NoGamespyVietcong v1.0
 * by Pavel Kalaš 2025 (Floxen).
 * -----------------------------
 * https://github.com/pavelkalas/NoGamespyVietcong
 */

namespace NoGamespyVietcong
{
    class EmbedExtractor
    {
        private readonly Random random = new Random();

        private readonly string tempExtractDirectory;

        public EmbedExtractor()
        {
            tempExtractDirectory = Path.GetTempPath() + GetRandomSubdirName();

            while (Directory.Exists(tempExtractDirectory))
            {
                Thread.Sleep(10);
            }

            CreateTempDirectory();
        }

        public string ExtractToDirectory(string embedFile)
        {
            string outputPath = Path.Combine(tempExtractDirectory, embedFile);

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("NoGamespyVietcong.Resources." + embedFile))
            {
                if (stream != null)
                {
                    using (FileStream fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                    {
                        stream.CopyTo(fs);
                    }
                }
            }

            File.SetAttributes(outputPath, FileAttributes.Hidden | FileAttributes.System);

            return outputPath;
        }

        private string GetRandomSubdirName()
        {
            string name = "vc$";

            for (int i = 0; i < 5; i++)
            {
                name += random.Next(1000, 9999).ToString();
            }

            return name;
        }

        private void CreateTempDirectory()
        {
            if (!Directory.Exists(tempExtractDirectory))
            {
                Directory.CreateDirectory(tempExtractDirectory);
                File.SetAttributes(tempExtractDirectory, FileAttributes.Hidden | FileAttributes.System);
            }
        }

        public void DeleteTempDirectory()
        {
            if (Directory.Exists(tempExtractDirectory))
            {
                File.SetAttributes(tempExtractDirectory, FileAttributes.Normal);
                Directory.Delete(tempExtractDirectory, true);
            }
        }

        public void DeleteExtractedFile(string extractedFile)
        {
            if (File.Exists(extractedFile))
            {
                File.SetAttributes(extractedFile, FileAttributes.Normal);
                File.Delete(extractedFile);
            }
        }
    }
}
