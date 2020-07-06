using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Plugin.Pushover.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Services;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Pushover.Api
{
    [Route("/Notification/Pushover/Test/{UserId}", "POST", Summary = "Tests Pushover")]
    public class TestNotification : IReturnVoid
    {
        [ApiMember(Name = "UserID", Description = "User Id", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string UserId { get; set; }
    }

    public class ServerApiEndpoints : IService
    {
        private readonly IHttpClient _httpClient;
        private readonly ILogger<TestNotification> _logger;
        private readonly IJsonSerializer _jsonSerializer;

        public ServerApiEndpoints(ILoggerFactory loggerFactory, IHttpClient httpClient, IJsonSerializer jsonSerializer)
        {
            _logger = loggerFactory.CreateLogger<TestNotification>();
            _httpClient = httpClient;
            _jsonSerializer = jsonSerializer;
        }
        private static PushOverOptions GetOptions(string userId)
        {
            return Plugin.Instance.Configuration.Options
                .FirstOrDefault(i => string.Equals(i.UserId, userId, StringComparison.OrdinalIgnoreCase));
        }

        public void Post(TestNotification request)
        {
            PostAsync(request)
                .GetAwaiter()
                .GetResult();
        }

        private async Task PostAsync(TestNotification request)
        {
            var options = GetOptions(request.UserId);

            var body = new Dictionary<string, string>
            {
                {"token", options.Token},
                {"user", options.UserKey},
                {"title", "Test Notification"},
                {"message", "This is a test notification from Jellyfin"}
            };

            _logger.LogDebug("Pushover <TEST> to {0} - {1}", options.Token, options.UserKey);

            var requestOptions = new HttpRequestOptions
            {
                Url = PluginConfiguration.Url,
                RequestContent = _jsonSerializer.SerializeToString(body),
                RequestContentType = "application/json",
                LogErrorResponseBody = true
            };

            await _httpClient.Post(requestOptions).ConfigureAwait(false);
        }
    }
}
