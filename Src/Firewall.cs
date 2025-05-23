using System.Diagnostics;

namespace NoGamespyVietcong.Src
{
    class Firewall
    {
        /// <summary>
        /// Přidává pravidlo pro hru do firewallu
        /// </summary>
        /// <param name="ruleName">Jméno pravidla</param>
        /// <param name="programPath">Cesta ke hře</param>
        public static void AddFirewallRule(string ruleName, string programPath)
        {
            var powerShellInterface = new ProcessStartInfo("netsh", $"advfirewall firewall add rule name=\"{ruleName}\" dir=in action=allow program=\"{programPath}\" enable=yes")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
            };

            var process = Process.Start(powerShellInterface);
            process.WaitForExit();
        }

        /// <summary>
        /// Odstraňuje pravidlo hry z firewallu
        /// </summary>
        /// <param name="ruleName">Jméno pravidla</param>
        public static void RemoveFirewallRule(string ruleName)
        {
            var powerShellInterface = new ProcessStartInfo("netsh", $"advfirewall firewall delete rule name=\"{ruleName}\"")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
            };

            var process = Process.Start(powerShellInterface);
            process.WaitForExit();
        }
    }
}
