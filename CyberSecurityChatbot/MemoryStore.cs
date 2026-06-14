using System.Collections.Generic;

namespace CybersecurityChatbot
{
    internal static class MemoryStore
    {
        internal static string UserName { get; set; } = "Friend";
        internal static string? FavouriteTopic { get; private set; }
        internal static string? LastTopic { get; set; }

        private static readonly Dictionary<string, int> TopicMentionCount = new();

        internal static void RecordTopicMention(string topicKey)
        {
            if (!TopicMentionCount.ContainsKey(topicKey))
                TopicMentionCount[topicKey] = 0;
            TopicMentionCount[topicKey]++;
            LastTopic = topicKey;

            string? mostMentioned = null;
            int max = 0;
            foreach (var kvp in TopicMentionCount)
                if (kvp.Value > max) { max = kvp.Value; mostMentioned = kvp.Key; }
            FavouriteTopic = mostMentioned;
        }

        internal static string? GetPersonalisedContext(string topicKey)
        {
            if (FavouriteTopic == topicKey &&
                TopicMentionCount.TryGetValue(topicKey, out int count) && count >= 2)
            {
                return $"  As someone who is interested in " +
                       $"{ResponseEngine.GetTopicDisplayName(topicKey)}, " +
                       $"here is another tip you might find useful:\n\n";
            }
            return null;
        }

        internal static string? GetInterestAcknowledgement(string input, string topicKey)
        {
            string lower = input.ToLowerInvariant();
            if (lower.Contains("interested in") || lower.Contains("want to know about") ||
                lower.Contains("worried about") || lower.Contains("concerned about") ||
                lower.Contains("tell me about") || lower.Contains("care about"))
            {
                return $"  Great — I will remember that you are interested in " +
                       $"{ResponseEngine.GetTopicDisplayName(topicKey)}! " +
                       $"It is a crucial part of staying safe online. Here is what you should know:\n\n";
            }
            return null;
        }

        internal static bool HasFavouriteTopic() => FavouriteTopic != null;
    }
}
