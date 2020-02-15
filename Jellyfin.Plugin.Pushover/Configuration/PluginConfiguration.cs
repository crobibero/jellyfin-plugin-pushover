using System.Collections.Generic;
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.Pushover.Configuration
{
    /// <summary>
    /// Class PluginConfiguration
    /// </summary>
    public class PluginConfiguration : BasePluginConfiguration
    {
        public const string Url = "https://api.pushover.net/1/messages.json";
        public PushOverOptions[] Options { get; set; }

        public PluginConfiguration()
        {
            Options = new PushOverOptions[] { };
        }
    }

    public class PushOverOptions
    {
        public bool Enabled { get; set; }
        public string UserKey { get; set; }
        public string Token { get; set; }
        public string DeviceName { get; set; }
        public List<Sound> SoundList { get; set; }
        public int Priority { get; set; }
        public string UserId { get; set; }

        public PushOverOptions()
        {
            SoundList = new List<Sound>
            {
                new Sound {Name = "Pushover", Value = "pushover"},
                new Sound {Name = "Bike", Value = "bike"},
                new Sound {Name = "Bugle", Value = "bugle"}
            };
        }
    }

    public class Sound
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
