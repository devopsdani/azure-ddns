namespace AzureDDNS.Poco
{
    internal class IpAddress
    {
        public string Name { get; set; }
        public string Ipv4Address { get; set; }

        public IpAddress(string name, string ipv4)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Ipv4Address = ipv4 ?? throw new ArgumentNullException(nameof(ipv4));
        }
    }
}
