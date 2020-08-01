using System;
using MarauderLib.Objects;

namespace MarauderLib.Services
{
    public class Logging
    {
        public static void Write(string source, string message)
        {
            if (State.Debug) {
                string time = DateTime.Now.ToString("o");
                Console.WriteLine("({0}) [{1}] - {2}", time, source, message);
            }
        }
    }
}
