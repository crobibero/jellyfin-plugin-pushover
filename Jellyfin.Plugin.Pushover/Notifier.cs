using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Pushover.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Notifications;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Pushover
{
    public class Notifier : INotificationService
    {
        private readonly ILogger _logger;
        private readonly IHttpClient _httpClient;
        private readonly IJsonSerializer _jsonSerializer;

        public Notifier(ILoggerFactory logManager, IHttpClient httpClient, IJsonSerializer jsonSerializer)
        {
            _logger = logManager.CreateLogger<Notifier>();
            _httpClient = httpClient;
            _jsonSerializer = jsonSerializer;
        }

        public bool IsEnabledForUser(User user)
        {
            var options = GetOptions(user);

            return options != null && IsValid(options) && options.Enabled;
        }

        private static PushOverOptions GetOptions(BaseItem user)
        {
            return Plugin.Instance.Configuration.Options
                .FirstOrDefault(i => string.Equals(i.UserId, user.Id.ToString("N"), StringComparison.OrdinalIgnoreCase));
        }

        public string Name => Plugin.Instance.Name;

        public async Task SendNotification(UserNotification request, CancellationToken cancellationToken)
        {
            var options = GetOptions(request.User);
            
            var body = new Dictionary<string, string>
            {
                {"token", options.Token},
                {"user", options.UserKey}
            };
            
            if (!string.IsNullOrEmpty(options.DeviceName))
                body.Add("device", options.DeviceName);

            if (string.IsNullOrEmpty(request.Description))
                body.Add("message", request.Name);
            else
            {
                body.Add("title", request.Name);
                body.Add("message", request.Description);
            }

            _logger.LogDebug("PushOver to Token : {0} - {1} - {2}", options.Token, options.UserKey, request.Description);

            var requestOptions = new HttpRequestOptions
            {
                Url = PluginConfiguration.Url,
                RequestContent = _jsonSerializer.SerializeToString(body),
                RequestContentType = "application/json",
                LogErrorResponseBody = true
            };

            await _httpClient.Post(requestOptions).ConfigureAwait(false);
        }

        private static bool IsValid(PushOverOptions options)
        {
            return !string.IsNullOrEmpty(options.UserKey) &&
                !string.IsNullOrEmpty(options.Token);
        }
    }
}
