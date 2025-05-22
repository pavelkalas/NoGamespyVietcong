namespace NoGamespyVietcong
{
    class Master
    {
        public string IpAddress;
        public long Ping;

        public Master(string ipAddress, long ping = -1)
        {
            this.IpAddress = ipAddress;
            this.Ping = ping;
        }
    }
}
