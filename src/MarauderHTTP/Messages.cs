using System.Linq;
using static MarauderHTTP.Program;

namespace MarauderHTTP
{
    internal static class Message
    {
        /// <summary>
        ///     Takes a string and converts it to an agent response as determined by the profile.
        /// </summary>
        public static string Render(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return Configuration.Profile.MessageConfig.Default;
            }
            return $"{Configuration.Profile.MessageConfig.Prepend}{message}{Configuration.Profile.MessageConfig.Append}";
        }

        /// <summary>
        ///     Used to extract the agent message from a HTTP relay response.
        /// </summary>
        internal static string Extract(string content)
        {
            var factionMessage = "";
            if (content != Configuration.Profile.ServerMessageConfig.Default)
            {
                factionMessage = content.Remove(0, Configuration.Profile.ServerMessageConfig.Prepend.Count());
                factionMessage =
                    factionMessage.Remove(factionMessage.Length -
                                          Configuration.Profile.ServerMessageConfig.Append.Count());
            }

            return factionMessage;
        }
    }
}