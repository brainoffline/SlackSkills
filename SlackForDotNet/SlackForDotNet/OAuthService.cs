using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using Asciis;
using Microsoft.Extensions.Logging;


namespace SlackForDotNet
{
    internal class OAuthService : IDisposable
    {
        public CancellationTokenSource CancellationSource { get; }
        public string? Code { get; private set; }
        public string? Error { get; private set; }
        public string? State { get; private set; }

        private ILogger<SlackApp>? Logger { get; }

        private TaskCompletionSource<bool>? _tcs;
        private HttpListener? _httpListener;

        public OAuthService(ILogger<SlackApp>? logger)
        {
            Logger = logger;
            CancellationSource = new CancellationTokenSource();

            _httpListener = new HttpListener();
            _httpListener.Prefixes.Clear();
        }

        void LaunchBrowserForOAuth(
            string clientId,
            string redirectUrl,
            string? botScopes = null,
            string? userScopes = null)
        {
            var state = Guid.NewGuid().ToString();
            var url = $"https://slack.com/oauth/v2/authorize?client_id={clientId}&redirect_uri={redirectUrl}&state={state}";
            if (!string.IsNullOrWhiteSpace(botScopes))
                url += $"&scope={HttpUtility.UrlEncode(botScopes)}";
            if (!string.IsNullOrWhiteSpace(userScopes))
                url += $"&user_scope={HttpUtility.UrlEncode(userScopes)}";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //url = url.Replace("&", "^&", StringComparison.InvariantCultureIgnoreCase);
                Process.Start( new ProcessStartInfo( url ) { UseShellExecute = true } );
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else if (RuntimeInformation.IsOSPlatform( OSPlatform.Linux ))
            {
                Process.Start("xdg-open", url);
            }
        }

        public Task RetrieveTokens(string clientId,
                                   string redirectUrl,
                                   string? botScopes = null,
                                   string? userScopes = null)
        {
            _tcs = new TaskCompletionSource<bool>();

            _ = Task.Run(() =>
                         {
                             if (_httpListener == null)
                                 return;

                             try
                             {
                                 _httpListener.Prefixes.Clear();
                                 _httpListener.Prefixes.Add(redirectUrl);
                                 _httpListener.Start();

                                 LaunchBrowserForOAuth(clientId, redirectUrl, botScopes, userScopes);

                                 while (true)
                                 {
                                     var context = _httpListener.GetContext();
                                     var request = context.Request;
                                     var response = context.Response;

                                     string contents;
                                     using (var readStream = request.InputStream)
                                     {
                                         using var sr = new StreamReader(readStream, Encoding.UTF8);
                                         contents = sr.ReadToEnd();
                                     }

                                     Logger?.LogInformation($"Request for {request.Url}");

                                     if (!contents.IsNullOrWhiteSpace())
                                         Logger?.LogTrace(contents);

                                     Code = request.QueryString["code"];
                                     Error = request.QueryString["error"];
                                     State = request.QueryString["state"];

                                     if (Code!.HasValue() || Error!.HasValue())
                                     {
                                         var cannedResponse = $"<html><body><p>{Error}</p><p>You can now close the browser</p></body></html>";

                                         response.ContentLength64 = cannedResponse.Length;

                                         using var os = response.OutputStream;
                                         using var w = new StreamWriter(os);

                                         w.Write(cannedResponse);
                                         w.Flush();

                                         break;
                                     }
                                     response.StatusCode = 404;
                                     response.StatusDescription = "";
                                     response.Close();
                                 }

                             }
                             catch (HttpListenerException hlex)
                             {
                                 Logger?.LogError(hlex, $"{nameof(OAuthService)} listening for response");
                             }
                             finally
                             {
                                 _tcs.SetResult(true);
                             }
                         }, CancellationSource.Token);

            return _tcs.Task;
        }

        public void Dispose()
        {
            _httpListener = null;
            CancellationSource.Dispose();
        }
    }
}
