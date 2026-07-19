using System.Text.Json;
using Gradio.Net.Core.Tunneling;

namespace Gradio.Net
{
    public static class Networking
    {
        private const string GradioApiServer = "https://api.gradio.app/v3/tunnel-request";
        private const string CertificatePath = ".gradio/certificate.pem";
        private static readonly string? GradioShareServerAddress = Environment.GetEnvironmentVariable("GRADIO_SHARE_SERVER_ADDRESS");

        public static string SetupTunnel(
            string localHost,
            int localPort,
            string shareToken,
            string? shareServerAddress,
            string? shareServerTlsCertificate
        )
        {
            shareServerAddress = string.IsNullOrEmpty(shareServerAddress)
                ? GradioShareServerAddress
                : shareServerAddress;

            string remoteHost = string.Empty;
            int remotePort = 0;

            if (string.IsNullOrEmpty(shareServerAddress))
            {
                try
                {
                    using var client = new HttpClient();
                    client.Timeout = TimeSpan.FromSeconds(30);
                    var response = client.GetAsync(GradioApiServer).GetAwaiter().GetResult();
                    response.EnsureSuccessStatusCode();

                    var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    using var doc = JsonDocument.Parse(body);
                    var root = doc.RootElement;
                    if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
                    {
                        throw new InvalidOperationException("Unexpected tunnel-request payload");
                    }

                    var payload = root[0];
                    remoteHost = payload.GetProperty("host").GetString() ?? string.Empty;
                    remotePort = payload.GetProperty("port").GetInt32();
                    var certificate = payload.GetProperty("root_ca").GetString() ?? string.Empty;

                    var certificateDir = Path.GetDirectoryName(CertificatePath);
                    if (!string.IsNullOrEmpty(certificateDir))
                    {
                        Directory.CreateDirectory(certificateDir);
                    }
                    File.WriteAllText(CertificatePath, certificate);
                    shareServerTlsCertificate = CertificatePath;

                    if (string.IsNullOrWhiteSpace(remoteHost) || remotePort <= 0)
                    {
                        throw new InvalidOperationException("Invalid tunnel host/port from Gradio API Server.");
                    }
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(
                        "Could not get share link from Gradio API Server.",
                        e
                    );
                }
            }
            else
            {
                var parts = shareServerAddress.Split(':');
                if (parts.Length != 2)
                {
                    throw new ArgumentException("share_server_address must be in the format 'host:port'");
                }
                remoteHost = parts[0];
                remotePort = int.Parse(parts[1]);
            }

            var tunnel = new Tunnel(
                remoteHost,
                remotePort,
                localHost,
                localPort,
                shareToken,
                shareServerTlsCertificate
            );
            var address = tunnel.StartTunnel();
            return address;
        }

        public static bool UrlOk(string url)
        {
            try
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                };
                using var client = new HttpClient(handler);
                client.Timeout = TimeSpan.FromSeconds(3);
                client.DefaultRequestHeaders.Add("User-Agent", "gradio");

                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        var response = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url)).GetAwaiter().GetResult();
                        var statusCode = (int)response.StatusCode;
                        if (statusCode == 200 || statusCode == 401 || statusCode == 302 || statusCode == 303 || statusCode == 307)
                        {
                            return true;
                        }
                    }
                    catch (HttpRequestException)
                    {
                        // ignore and retry
                    }
                    catch (TaskCanceledException)
                    {
                        // timeout, ignore and retry
                    }

                    Task.Delay(500).GetAwaiter().GetResult();
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
    }
}
