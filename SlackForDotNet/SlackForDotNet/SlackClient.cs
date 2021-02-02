using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using System.Threading.Tasks;
using System.Web;

using Asciis;

using Microsoft.Extensions.Logging;

#if SYSTEM_JSON
using System.Text.Json;
#else
using Newtonsoft.Json;
#endif

using SlackForDotNet.WebApiContracts;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SlackForDotNet
{
    /// <summary>
    /// Slack client for calling Slack WebAPI
    /// </summary>
    public class SlackClient
    {
        const string BaseSlackApi = "https://slack.com/api/";

        private readonly HttpClient         _httpClient;
        private          ILogger<SlackApp>? Logger { get; }

        public SlackClient( ILogger< SlackApp >? logger )
        {
            Logger = logger;
            _httpClient = new HttpClient(
                new HttpClientHandler { DefaultProxyCredentials = CredentialCache.DefaultCredentials } );
        }

        public async Task<OAuthAccessResponse2?> OAuthAccess(
            string clientId,
            string clientSecret,
            string code,
                string redirectUrl
            )
        {
            return await PostUnAuth< OAuthAccess2, OAuthAccessResponse2 >( new OAuthAccess2
                                                                         {
                                                                             client_id = clientId,
                                                                             client_secret = clientSecret,
                                                                             code = code,
                                                                             redirect_uri = redirectUrl
                                                                         } );
        }
        
        public Task< TResponse? > PostUnAuth< TRequest, TResponse >( TRequest request )
            where TRequest : SlackMessage
            where TResponse : MessageResponse
        {
            return PostMessage<TRequest, TResponse>(null, request);
        }

        /// <summary>
        /// Post message
        /// </summary>
        public Task< TResponse? > Post< TRequest, TResponse >( string          token,
                                                                     TRequest? request )
            where TRequest : SlackMessage
            where TResponse : MessageResponse
        {
            return PostMessage< TRequest, TResponse >( token, request );
        }

        /// <summary>
        /// Post message
        /// </summary>
        private async Task<TResponse?> PostMessage<TRequest, TResponse>(
            string?   token,
            TRequest? request)
            where TRequest : SlackMessage
            where TResponse : MessageResponse
        {
            var messageAttribute = MessageTypes.GetMessageAttributes<TRequest>();
            if (request != null)
            {
                request.type    = messageAttribute.Type;
                request.subtype = messageAttribute.SubType;
            }

            var uri = GetSlackUri(messageAttribute.Type);

            bool postJson = messageAttribute.ApiType.HasFlag(Msg.Json)
                || !messageAttribute.ApiType.HasFlag(Msg.FormEncoded);

            Logger.LogInformation($">> {uri}");
            HttpRequestMessage webRequest = new HttpRequestMessage(HttpMethod.Post, uri);
            if (postJson)
            {
#if SYSTEM_JSON
                var content = JsonSerializer.Serialize(request, JsonHelpers.DefaultJsonOptions);
#else
                var content = JsonConvert.SerializeObject(request);
#endif
                Logger.LogDebug($">> {content}");

                webRequest.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }
            else
            {
                var parameters = new Parameters(request);
                Logger.LogDebug($">> {parameters}");

                webRequest.Content = new FormUrlEncodedContent(parameters);
            }
            if (!string.IsNullOrWhiteSpace( token ))
                webRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var webResponse = await _httpClient.SendAsync(webRequest);
            var json = await webResponse.Content.ReadAsStringAsync();

            Logger.LogDebug($"<< {json}");

#if SYSTEM_JSON
            return JsonSerializer.Deserialize<TResponse>(json);
#else
            return JsonConvert.DeserializeObject<TResponse>(json);
#endif
        }

        /// <summary>
        /// Send WebApi GET request
        /// </summary>
        public Task<TResponse?> GetUnauth<TRequest,TResponse>(TRequest? request = default)
            where TRequest : SlackMessage
            where TResponse : MessageResponse
        {
            return GetMessage<TRequest,TResponse>(null, request);
        }

        /// <summary>
        /// Send WebApi GET request
        /// </summary>
        public Task< TResponse? > GetRequest< TRequest, TResponse >( string token, TRequest? request = default )
            where TRequest : SlackMessage
            where TResponse : MessageResponse
        {
            return GetMessage< TRequest,TResponse >( token, request );
        }

        /// <summary>
        /// Send WebApi GET request
        /// </summary>
        public Task<TResponse?> GetRequest<TRequest, TResponse>(string token, Parameters queryParameters)
            where TRequest : SlackMessage
            where TResponse : MessageResponse
        {
            return GetMessage<TRequest, TResponse>(token, null, queryParameters);
        }

        /// <summary>
        /// Send WebApi GET request
        /// </summary>
        private async Task<TResponse?> GetMessage<TRequest,TResponse>(string? token, TRequest? request = null, Parameters? queryParameters = default)
            where TRequest : SlackMessage
            where TResponse : MessageResponse
        {
            var responseAttrs = MessageTypes.GetMessageAttributes<TRequest>();

            queryParameters ??= new Parameters(request);

            var uri = GetSlackUri(responseAttrs.Type, queryParameters);

            Logger.LogInformation($">> {uri}");

            var webRequest = (HttpWebRequest)WebRequest.Create(uri);
            if (!string.IsNullOrWhiteSpace(token))
                webRequest.Headers["Authorization"] = "Bearer " + token;

            HttpWebResponse response = (HttpWebResponse)await webRequest.GetResponseAsync()
                                                                        .ConfigureAwait(true);

            await using var stream = response.GetResponseStream();

            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync().ConfigureAwait(true);

            Logger.LogDebug($"<< {json}");

#if SYSTEM_JSON
            return JsonSerializer.Deserialize<TResponse>(json);
#else
            return JsonConvert.DeserializeObject<TResponse>(json);
#endif

        }

        protected Uri GetSlackUri(string path, Parameters? getParameters = null)
        {
            if (getParameters?.Count > 0)
            {
                var args = HttpUtility.ParseQueryString("");
                foreach (var pair in getParameters)
                    args[pair.Key] = pair.Value?.ToString();
                return new Uri(Path.Combine(BaseSlackApi, path) + "?" + args);
            }

            return new Uri(Path.Combine(BaseSlackApi, path));
        }

    }
}
