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
        /// <summary>
        /// Instance pro generování random čísel
        /// </summary>
        private readonly Random random = new Random();

        /// <summary>
        /// Cesta k dočasné složce pro extrakci
        /// </summary>
        private readonly string tempExtractDirectory;

        public readonly string randomSeedNumber;

        public EmbedExtractor()
        {
            tempExtractDirectory = Path.GetTempPath() + GetRandomSubdirName(ref randomSeedNumber);

            while (Directory.Exists(tempExtractDirectory))
            {
                Thread.Sleep(10);
            }

            CreateTempDirectory();
        }

        /// <summary>
        /// Extrahuje určitý soubor z Embed resources do dočasné složky
        /// </summary>
        /// <param name="embedFile">Jméno embed souboru ve složce Resources</param>
        /// <returns>Vrací extrahovanou cestu</returns>
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

        /// <summary>
        /// Vygeneruje náhodné jméno pro podsložku v %TEMP%
        /// </summary>
        /// <returns>Náhodné jméno</returns>
        private string GetRandomSubdirName(ref string randomSeedNumber)
        {
            string name = "vc$";

            for (int i = 0; i < 5; i++)
            {
                string nums = random.Next(1000, 9999).ToString();
                randomSeedNumber += nums;
                name += nums;
            }

            return name;
        }

        /// <summary>
        /// Vytvoří dočasnou složku
        /// </summary>
        private void CreateTempDirectory()
        {
            if (!Directory.Exists(tempExtractDirectory))
            {
                Directory.CreateDirectory(tempExtractDirectory);
                File.SetAttributes(tempExtractDirectory, FileAttributes.Hidden | FileAttributes.System);
            }
        }

        /// <summary>
        /// Smaže dočasnou složku
        /// </summary>
        public void DeleteTempDirectory()
        {
            if (Directory.Exists(tempExtractDirectory))
            {
                File.SetAttributes(tempExtractDirectory, FileAttributes.Normal);
                Directory.Delete(tempExtractDirectory, true);
            }
        }

        /// <summary>
        /// Smaže extrahovaný soubor
        /// </summary>
        /// <param name="extractedFile"></param>
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
