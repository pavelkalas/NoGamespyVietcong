using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

/**
 * NoGamespyVietcong v1.0
 * by Pavel Kalaš 2025 (Floxen).
 * -----------------------------
 * https://github.com/pavelkalas/NoGamespyVietcong
 */

namespace NoGamespyVietcong.Src.MS
{
    class Masterserver
    {
        /// <summary>
        /// Instance pingování
        /// </summary>
        private static readonly Ping ping = new Ping();

        /// <summary>
        /// List funkčních masterserverů
        /// </summary>
        private static readonly List<Master> masterList = new List<Master>()
        {
            new Master("46.28.109.117"),         // vietcong1.eu  MASTER
            new Master("85.255.3.25"),           // alpha-team.cz MASTER
        };

        /// <summary>
        /// Získá nejlepší masterserver podle odezvy
        /// </summary>
        /// <returns>Vrací IP masterserveru s nejlepší odezvou</returns>
        public static string GetBestConnectionMaster()
        {
            foreach (var master in masterList)
            {
                lock (masterList)
                {
                    long pingResult = PingMasterServer(master.IpAddress);

                    lock (master)
                    {
                        master.Ping = pingResult;
                    }
                }
            }

            // vymaže nedostupné masterservery
            masterList.RemoveAll(mst => mst.Ping == 0);

            // navrátí nejlepší master podle odezvy
            return masterList.OrderBy(mst => mst.Ping).ToList().FirstOrDefault().IpAddress;
        }

        /// <summary>
        /// Pingne masterserver a navrátí odezvu
        /// </summary>
        /// <param name="ipAddress">IP adresa masterserveru</param>
        /// <param name="timeOut">Maximální čas snažit se o pingnutí</param>
        /// <returns>Odezva masterserveru</returns>
        private static long PingMasterServer(string ipAddress, int timeOut = 2000)
        {
            return ping.Send(ipAddress, timeOut).RoundtripTime;
        }
    }
}
