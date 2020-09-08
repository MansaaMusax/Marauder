using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace MarauderHTTP.Common
{
    public class AgentIdentifier
    {
        public string Location;
        public string Name;
    }

    public class MessageConfig
    {
        // This is the default response when there is no payload.
        public string Default;

        // This text will be added before the base64 encoded 
        // payload string
        public string Prepend;

        // This text will be added after the base64 encoded
        // payload string
        public string Append;
    }

    public class Profile
    {
        // Host to connect to
        public List<string> Hosts;

        // URLs to use
        public List<string> URLs;

        // List of headername:values that will be added to
        // each response
        public Dictionary<string, string> Headers;

        // List of cookiename:values that will be added to
        // each response
        public Dictionary<string, string> Cookies;
        public string PayloadName;
        public AgentIdentifier CheckinIdentifier;
        public AgentIdentifier StagingIdentifier;
        public MessageConfig MessageConfig;
        public MessageConfig ServerMessageConfig;
    }


    public class Configuration
    {
        internal string PayloadName;
        internal string PayloadKey;
        internal int Sleep;
        internal double Jitter;
        internal int MaxAttempts;
        internal DateTime? ExpirationDate;
        internal bool Debug;
        internal Profile Profile;

        public Configuration()
        {
            var settingsStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MarauderHTTP.settings.json");
            JsonConvert.DeserializeObject<Configuration>(new StreamReader(settingsStream ?? throw new InvalidOperationException()).ReadToEnd());
        }
    }
}