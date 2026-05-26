using System;
using System.IO;
using System.Media;

namespace CybersecurityChatbot
{
    internal static class VoiceGreeting
    {
        private static readonly string WavPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Resources",
            "greeting.wav");

        /// <summary>
        /// Plays the WAV greeting asynchronously so the GUI
        /// loads at the same time as audio plays.
        /// Falls back to beeps if the file is missing.
        /// </summary>
        internal static void Play()
        {
            try
            {
                if (File.Exists(WavPath))
                {
                    using SoundPlayer player = new(WavPath);
                    player.Play(); // async — does not block the UI thread
                }
                else
                {
                    Console.Beep(523, 200);
                    Console.Beep(659, 200);
                    Console.Beep(784, 400);
                }
            }
            catch
            {
                // Never crash because of audio
            }
        }
    }
}