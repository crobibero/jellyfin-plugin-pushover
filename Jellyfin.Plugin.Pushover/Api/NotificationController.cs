using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Jellyfin.Plugin.Pushover.Configuration;
using MediaBrowser.Common.Json;
using MediaBrowser.Common.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Pushover.Api
{
    [Route("Notification/Pushover")]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public NotificationController(
            ILogger<NotificationController> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _jsonSerializerOptions = JsonDefaults.GetOptions();
        }

        [HttpPost("Test/{userId}")]
        public async Task<ActionResult> Test([FromRoute] Guid userId)
        {
            var options = GetOptions(userId);

            var body = new Dictionary<string, string>
            {
                {"token", options.Token},
                {"user", options.UserKey},
                {"title", "Test Notification"},
                {"message", "This is a test notification from Jellyfin"}
            };

            _logger.LogDebug("Pushover <TEST> to {0} - {1}", options.Token, options.UserKey);
            
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, PluginConfiguration.Url);
            requestMessage.Content = new StringContent(
                JsonSerializer.Serialize(body, _jsonSerializerOptions),
                Encoding.UTF8,
                MediaTypeNames.Application.Json);

            using var response = await _httpClientFactory.CreateClient(NamedClient.Default)
                .SendAsync(requestMessage)
                .ConfigureAwait(false);

            return NoContent();
        }
        
        private static PushOverOptions GetOptions(Guid userId)
        {
            return Plugin.Instance.Configuration.Options
                .FirstOrDefault(i => Guid.Parse(i.UserId) == userId);
        }
    }
}