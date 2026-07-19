using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Text;
using Gradio.Net.Core.Exceptions;

namespace Gradio.Net.Core.Tunneling
{
    public class Tunnel
    {
        public static readonly string VERSION = "0.3";
        public static readonly List<Tunnel> CURRENT_TUNNELS = new();
        private static readonly string HF_HOME = Environment.GetEnvironmentVariable("HF_HOME") ??
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cache", "huggingface");
        private static readonly string BINARY_REMOTE_NAME;
        private static readonly string EXTENSION;
        private static readonly string BINARY_URL;
        private static readonly string BINARY_FILENAME;
        private static readonly string BINARY_FOLDER;
        private static readonly string BINARY_PATH;
        private const int TUNNEL_TIMEOUT_SECONDS = 30;
        private const string TUNNEL_ERROR_MESSAGE =
            "Could not create share URL. " +
            "Please check the appended log from frpc for more information:";
        private const string CERTIFICATE_PATH = ".gradio/certificate.pem";
        private const int CHUNK_SIZE = 128;
        private static readonly Dictionary<string, string> CHECKSUMS = new()
        {
            { "https://cdn-media.huggingface.co/frpc-gradio-0.3/frpc_windows_amd64.exe", "14bc0ea470be5d67d79a07412bd21de8a0a179c6ac1116d7764f68e942dc9ceb" },
            { "https://cdn-media.huggingface.co/frpc-gradio-0.3/frpc_linux_amd64", "c791d1f047b41ff5885772fc4bf20b797c6059bbd82abb9e31de15e55d6a57c4" },
            { "https://cdn-media.huggingface.co/frpc-gradio-0.3/frpc_linux_arm64", "823ced25104de6dc3c9f4798dbb43f20e681207279e6ab89c40e2176ccbf70cd" },
            { "https://cdn-media.huggingface.co/frpc-gradio-0.3/frpc_darwin_amd64", "930f8face3365810ce16689da81b7d1941fda4466225a7bbcbced9a2916a6e15" },
            { "https://cdn-media.huggingface.co/frpc-gradio-0.3/frpc_darwin_arm64", "dfac50c690aca459ed5158fad8bfbe99f9282baf4166cf7c410a6673fbc1f327" },
            { "https://cdn-media.huggingface.co/frpc-gradio-0.3/frpc_linux_arm", "4b563beb2e36c448cc688174e20b53af38dc1ff2b5e362d4ddd1401f2affbfb7" },
            { "https://cdn-media.huggingface.co/frpc-gradio-0.3/frpc_freebsd_386", "cb0a56c764ecf96dd54ed601d240c564f060ee4e58202d65ffca17c1a51ce19c" },
            { "https://cdn-media.huggingface.co/frpc-gradio-0.3/frpc_freebsd_amd64", "516d9e6903513869a011ddcd1ec206167ad1eb5dd6640d21057acc258edecbbb" },
            { "https://cdn-media.huggingface.co/frpc-gradio-0.3/frpc_linux_386", "4c2f2a48cd71571498c0ac8a4d42a055f22cb7f14b4b5a2b0d584220fd60a283" },
            { "https://cdn-media.huggingface.co/frpc-gradio-0.3/frpc_linux_mips", "b309ecd594d4f0f7f33e556a80d4b67aef9319c00a8334648a618e56b23cb9e0" },
            { "https://cdn-media.huggingface.co/frpc-gradio-0.3/frpc_linux_mips64", "0372ef5505baa6f3b64c6295a86541b24b7b0dbe4ef28b344992e21f47624b7b" },
            { "https://cdn-media.huggingface.co/frpc-gradio-0.3/frpc_linux_riscv64", "1658eed7e8c14ea76e1d95749d58441ce24147c3d559381832c725c29cfc3df3" },
            { "https://cdn-media.huggingface.co/frpc-gradio-0.3/frpc_linux_mipsle", "a2aaba16961d3372b79bd7a28976fcd0f0bbaebc2b50d5a7a71af2240747960f" },
            { "https://cdn-media.huggingface.co/frpc-gradio-0.3/frpc_windows_386.exe", "721b90550195a83e15f2176d8f85a48d5a25822757cb872e9723d4bccc4e5bb6" },
            { "https://cdn-media.huggingface.co/frpc-gradio-0.3/frpc_linux_mips64le", "796481edd609f31962b45cc0ab4c9798d040205ae3bf354ed1b72fb432d796b8" }
        };

        static Tunnel()
        {
            string machine = RuntimeInformation.OSArchitecture.ToString().ToLower();
            if (machine == "x64")
            {
                machine = "amd64";
            }
            else if (machine == "arm64")
            {
                machine = "arm64";
            }

            string system = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "windows" :
                           RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" :
                           RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "darwin" :
                           "unknown";

            BINARY_REMOTE_NAME = $"frpc_{system}_{machine}";
            EXTENSION = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : "";
            BINARY_URL = $"https://cdn-media.huggingface.co/frpc-gradio-{VERSION}/{BINARY_REMOTE_NAME}{EXTENSION}";
            BINARY_FILENAME = $"{BINARY_REMOTE_NAME}_v{VERSION}";
            BINARY_FOLDER = Path.Combine(HF_HOME, "gradio", "frpc");
            BINARY_PATH = Path.Combine(BINARY_FOLDER, BINARY_FILENAME);
        }

        private Process? _proc;
        public string? Url { get; private set; }
        public string RemoteHost { get; }
        public int RemotePort { get; }
        public string LocalHost { get; }
        public int LocalPort { get; }
        public string ShareToken { get; }
        public string? ShareServerTlsCertificate { get; }

        public Tunnel(
            string remoteHost,
            int remotePort,
            string localHost,
            int localPort,
            string shareToken,
            string? shareServerTlsCertificate = null)
        {
            RemoteHost = remoteHost;
            RemotePort = remotePort;
            LocalHost = localHost;
            LocalPort = localPort;
            ShareToken = shareToken;
            ShareServerTlsCertificate = shareServerTlsCertificate;
        }

        public static void DownloadBinary()
        {
            if (!File.Exists(BINARY_PATH))
            {
                Directory.CreateDirectory(BINARY_FOLDER);
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);

                try
                {
                    var response = httpClient.GetAsync(BINARY_URL).Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        throw new IOException(
                            $"Cannot set up a share link as this platform is incompatible. Please " +
                            $"create a GitHub issue with information about your platform: {RuntimeInformation.OSDescription}");
                    }

                    response.EnsureSuccessStatusCode();

                    var content = response.Content.ReadAsByteArrayAsync().Result;
                    File.WriteAllBytes(BINARY_PATH, content);

                    // Make file executable on Unix-like systems
                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        Process.Start("chmod", $"+x {BINARY_PATH}")?.WaitForExit();
                    }

                    if (CHECKSUMS.TryGetValue(BINARY_URL, out var expectedHash))
                    {
                        using var sha256 = SHA256.Create();
                        using var stream = File.OpenRead(BINARY_PATH);
                        var calculatedHash = BitConverter.ToString(sha256.ComputeHash(stream))
                            .Replace("-", "").ToLower();

                        if (calculatedHash != expectedHash)
                        {
                            throw new ChecksumMismatchError();
                        }
                    }
                }
                catch (AggregateException ex) when (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }
            }
        }

        public string StartTunnel()
        {
            DownloadBinary();
            Url = _StartTunnel(BINARY_PATH);
            return Url;
        }

        public void Kill()
        {
            if (_proc != null && !_proc.HasExited)
            {
                try
                {
                    // Try graceful shutdown first
                    _proc.CloseMainWindow();
                    if (!_proc.WaitForExit(2000))
                    {
                        // Force kill if graceful shutdown failed
                        _proc.Kill();
                    }
                }
                catch (Exception)
                {
                    // Process may have already exited
                }
                finally
                {
                    _proc = null;
                }
            }
        }

        private string _StartTunnel(string binary)
        {
            CURRENT_TUNNELS.Add(this);

            var args = new List<string>
            {
                "http",
                "-n",
                ShareToken,
                "-l",
                LocalPort.ToString(),
                "-i",
                LocalHost,
                "--uc",
                "--sd",
                "random",
                "--ue",
                "--server_addr",
                $"{RemoteHost}:{RemotePort}",
                "--disable_log_color"
            };

            if (ShareServerTlsCertificate != null)
            {
                args.AddRange(new[]
                {
                    "--tls_enable",
                    "--tls_trusted_ca_file",
                    ShareServerTlsCertificate
                });
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = binary,
                Arguments = string.Join(" ", args),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            _proc = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start tunnel process");

            // Register for process exit
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => Kill();

            return _ReadUrlFromTunnelStream();
        }

        private string _ReadUrlFromTunnelStream()
        {
            DateTime startTime = DateTime.Now;
            var log = new List<string>();
            string url = string.Empty;

            void RaiseTunnelError()
            {
                string logText = string.Join("\n", log);
                Console.Error.WriteLine(logText);
                throw new InvalidOperationException($"{TUNNEL_ERROR_MESSAGE}\n{logText}");
            }

            while (string.IsNullOrEmpty(url))
            {
                if ((DateTime.Now - startTime).TotalSeconds >= TUNNEL_TIMEOUT_SECONDS)
                {
                    RaiseTunnelError();
                }

                if (_proc == null || _proc.HasExited)
                {
                    RaiseTunnelError();
                }

                if (_proc.StandardOutput is StreamReader stdout && stdout.Peek() > -1)
                {
                    string line = stdout.ReadLine() ?? string.Empty;
                    if (!string.IsNullOrEmpty(line))
                    {
                        log.Add(line.Trim());

                        if (line.Contains("start proxy success"))
                        {
                            var match = Regex.Match(line, @"start proxy success:\s*(.+)");
                            if (match.Success)
                            {
                                url = match.Groups[1].Value.Trim();
                            }
                            else
                            {
                                RaiseTunnelError();
                            }
                        }
                        else if (line.Contains("login to server failed"))
                        {
                            RaiseTunnelError();
                        }
                    }
                }
                else
                {
                    // Avoid busy-waiting
                    System.Threading.Thread.Sleep(100);
                }
            }

            return url;
        }
    }
}
