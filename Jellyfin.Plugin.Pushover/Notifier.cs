using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Pushover.Configuration;
using Jellyfin.Data.Entities;
using MediaBrowser.Common.Json;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Notifications;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Pushover
{
    public class Notifier : INotificationService
    {
        private readonly ILogger<Notifier> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _serializerOptions;

        public Notifier(ILoggerFactory logManager, IHttpClientFactory httpClientFactory)
        {
            _logger = logManager.CreateLogger<Notifier>();
            _httpClientFactory = httpClientFactory;
            _serializerOptions = JsonDefaults.GetOptions();
        }

        public bool IsEnabledForUser(User user)
        {
            var options = GetOptions(user);

            return options != null && IsValid(options) && options.Enabled;
        }

        private static PushOverOptions GetOptions(User user)
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

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, PluginConfiguration.Url);
            requestMessage.Content = new StringContent(
                JsonSerializer.Serialize(body, _serializerOptions),
                Encoding.UTF8,
                MediaTypeNames.Application.Json);

            using var response = await _httpClientFactory.CreateClient(NamedClient.Default)
                .SendAsync(requestMessage)
                .ConfigureAwait(false);
        }

        private static bool IsValid(PushOverOptions options)
        {
            return !string.IsNullOrEmpty(options.UserKey) &&
                !string.IsNullOrEmpty(options.Token);
        }
    }
}
