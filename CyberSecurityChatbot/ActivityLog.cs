
using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbot
{
    internal static class ActivityLog
    {
        private static readonly List<string> Log = new();
        private const int MaxDisplay = 10;

        /// <summary>Adds an entry to the activity log with a timestamp.</summary>
        internal static void Add(string action)
        {
            string entry = $"[{DateTime.Now:HH:mm:ss}]  {action}";
            Log.Add(entry);
        }

        /// <summary>Returns the last 10 log entries formatted for display.</summary>
        internal static string GetLog()
        {
            if (Log.Count == 0)
                return "  No activity recorded yet. Start chatting, add tasks, or take the quiz!";

            var recent = Log.TakeLast(MaxDisplay).ToList();
            string result = "  RECENT ACTIVITY LOG\n";
            result += new string('─', 50) + "\n";

            for (int i = 0; i < recent.Count; i++)
                result += $"{i + 1}. {recent[i]}\n";

            if (Log.Count > MaxDisplay)
                result += $"\n...and {Log.Count - MaxDisplay} more earlier actions.";

            return result;
        }

        /// <summary>Returns total number of logged actions.</summary>
        internal static int Count => Log.Count;
    }
}
