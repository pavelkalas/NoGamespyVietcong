using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

/**
 * NoGamespyVietcong v1.0
 * by Pavel Kalaš 2025 (Floxen).
 * -----------------------------
 * https://github.com/pavelkalas/NoGamespyVietcong
 */

namespace NoGamespyVietcong
{
    class Program
    {
        /// <summary>
        /// Instance na Extractor
        /// </summary>
        private static readonly EmbedExtractor embedExtractor = new EmbedExtractor();

        /// <summary>
        /// Hlavní vstupní bod
        /// </summary>
        /// <param name="args">Spouštěcí argumenty</param>
        static void Main(string[] args)
        {
            if (Directory.Exists("logs.dll") && Directory.Exists("game.dll"))
            {
                // extrahuje soubor vietcong.exe z /resources/ do %TEMP% a navrátí plnou cestu k souboru
                string vietcongPath = embedExtractor.ExtractToDirectory("vietcong.exe");

                // vytvoří process pro spuštění tohoto embed souboru
                Process process = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = vietcongPath,
                        UseShellExecute = true,
                        WorkingDirectory = Environment.CurrentDirectory,
                        Arguments = string.Join(" ", args),
                    }
                };

                // spustí hru
                if (process.Start())
                {
                    // počká na ukončení procesu
                    process.WaitForExit();

                    // uklidí po sobě extrahované soubory ve složce %TEMP%
                    embedExtractor.DeleteExtractedFile(vietcongPath);
                    embedExtractor.DeleteTempDirectory();
                }
            }
            else
            {
                MessageBox.Show("Make sure this file is inside your vietcong directory, because there is missing game.dll or logs.dll file.\n\nIf you have errors even when you think you are correct, contact me on discord: swipesznx6.", "vietcong.exe - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
        }
    }
}
