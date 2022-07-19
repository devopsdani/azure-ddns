using AzureDDNS.Model;
using AzureDDNS.Poco;
using System.Net;
using System.Text.Json;

namespace AzureDDNS
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            string? azurewebhook = null;
            string? key = null;
            string? subdomain = null;
            string? ipcheckuri = null;

            foreach (var arg in args)
            {
                if (!arg.Contains('='))
                {
                    throw new ArgumentException("Invalid argument. Required arguments: --azurewebhook=___ --key=___ --subdomain=___ --ipcheckuri=___");
                }

                var value = new string(arg.SkipWhile(c => c != '=').Skip(1).ToArray());
                _ = arg.Split('=')[0] switch
                {
                    "--azurewebhook" => azurewebhook = value,
                    "--key" => key = value,
                    "--subdomain" => subdomain = value,
                    "--ipcheckuri" => ipcheckuri = value,
                    _ => throw new NotImplementedException($"Unknown argument: {arg}")
                };
            }

            var azure = new AzureDNS(azurewebhook, key, subdomain, ipcheckuri);
            
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(5);

            IpAddress? ip = null;

            while(true)
            {
                var now = DateTimeOffset.UtcNow;
                var delay = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Offset)
                    .AddSeconds(10) - DateTimeOffset.UtcNow;
                await Task.Delay(delay);

                var ipv4 = await httpClient.GetStringAsync(ipcheckuri);
                if (ip is null || ipv4 != ip.Ipv4Address)
                {
                    var temp = new IpAddress(azure.Subdomain, ipv4);
                    var data = new StringContent(JsonSerializer.Serialize(temp));
                    data.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    try
                    {
                        var request = await httpClient.PostAsync(azure.WebhookUri + $"?code={azure.Key}", data);
                        Console.WriteLine(await request.Content.ReadAsStringAsync());
                        if (request.StatusCode == HttpStatusCode.OK)
                        {
                            ip = temp;
                            Console.WriteLine($"Successfully updated DNS record {azure.Subdomain} to {ipv4}!");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to updated DNS record {azure.Subdomain} to {ipv4}! STATUS CODE: {request.StatusCode} | RESPONSE {request.Content.ReadAsStringAsync()}");
                        }
                    }
                    catch (TimeoutException e)
                    {
                        Console.WriteLine($"Timeout occured while updating DNS record {azure.Subdomain} to {ipv4}! Retrying after 5 seconds...");
                    }
                }
            }
        }
    }
}