using NoGamespyVietcong.Src.Mem;
using NoGamespyVietcong.Src.MS;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
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
            if (File.Exists("logs.dll") && File.Exists("game.dll"))
            {
                string selectedMaster = string.Empty;

                // spustí task a získá nejlepší master podle jeho odezvy
                selectedMaster = Masterserver.GetBestConnectionMaster();

                // extrahuje soubor vietcong.exe z /resources/ do %TEMP% a navrátí plnou cestu k souboru
                string vietcongPath = embedExtractor.ExtractToDirectory("vietcong.exe");
                string injectorPath = embedExtractor.ExtractToDirectory("NoGamespyVietcongInjector.dll");

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
                    while (!process.MainWindowTitle.ToLower().EndsWith("vietcong") || process.MainWindowTitle.ToLower() == "vietcong")
                    {
                        process.Refresh();
                        Thread.Sleep(100);
                    }

                    Thread.Sleep(600);

                    Memory.InjectDllToProcess(process, injectorPath);
                    Memory.WriteObjectToAddress(process.Id, "logs.dll", 0x1890A4, selectedMaster);

                    new Thread(() =>
                    {
                        // počká na ukončení procesu
                        process.WaitForExit();

                        // uklidí po sobě extrahované soubory ve složce %TEMP%
                        embedExtractor.DeleteExtractedFile(vietcongPath);
                        embedExtractor.DeleteExtractedFile(injectorPath);
                        embedExtractor.DeleteTempDirectory();
                    }).Start();
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
