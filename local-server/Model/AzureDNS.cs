namespace AzureDDNS.Model
{
    internal class AzureDNS
    {
        public string WebhookUri { get; set; }
        public string Key { get; set;}
        public string Subdomain { get; set; }
        public string IpCheckUri { get; set; }

        public AzureDNS(string? webhookUri, string? key, string? subdomain, string? ipCheckUri)
        {
            WebhookUri = webhookUri ?? throw new ArgumentNullException(nameof(webhookUri));
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Subdomain = subdomain ?? throw new ArgumentNullException(nameof(subdomain));
            IpCheckUri = ipCheckUri ?? throw new ArgumentNullException(nameof(ipCheckUri));
        }
    }
}
