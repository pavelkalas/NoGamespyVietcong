/**
 * NoGamespyVietcong v1.0
 * by Pavel Kalaš 2025 (Floxen).
 * -----------------------------
 * https://github.com/pavelkalas/NoGamespyVietcong
 */

namespace NoGamespyVietcong.Src.MS
{
    class Master
    {
        /// <summary>
        /// IP adresa masterserveru
        /// </summary>
        public string IpAddress;

        /// <summary>
        /// Ping, response time serveru
        /// </summary>
        public long Ping;

        public Master(string ipAddress, long ping = -1)
        {
            this.IpAddress = ipAddress;
            this.Ping = ping;
        }
    }
}
