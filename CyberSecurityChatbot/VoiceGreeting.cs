using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Media;


namespace CybersecurityChatbot
{
    internal static class VoiceGreeting
    {
        // The file path is worked out based on where the program is running from
        private static readonly string WavPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Resources",
            "greeting.wav");

        /// <summary>
        /// Plays the WAV greeting 
        /// If the file is missing or playback fails, falls back to
        /// a short beep so the app never crashes.
        /// </summary>
        internal static void Play()
        {
            try
            {
                if (File.Exists(WavPath))
                {
                    using SoundPlayer player = new(WavPath);
                    player.PlaySync(); // waits until the audio finishes before UI appears
                }
                else
                {
                    
                    Console.Beep(523, 200);  // C5
                    Console.Beep(659, 200);  // E5
                    Console.Beep(784, 400);  // G5
                }
            }
            catch
            {
                // Audio errors must never crash the application
            }
        }
    }
}