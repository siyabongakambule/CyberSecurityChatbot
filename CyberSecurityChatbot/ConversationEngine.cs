namespace CybersecurityChatbot
{
    internal static class ConversationEngine
    {
        private static readonly string[] FollowUpPhrases =
        {
            "give me another tip", "another tip", "tell me more", "explain more",
            "more details", "more info", "continue", "go on", "keep going",
            "elaborate", "more please", "what else", "more", "another one",
            "give me more", "say more", "expand on that"
        };

        internal static string ProcessInput(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return "  Please type something — I am here to help!\nType 'help' to see available topics.";

            string input = userInput.Trim();

            // Step 1 — Detect sentiment
            Sentiment sentiment = SentimentDetector.Detect(input);
            string sentimentPrefix = SentimentDetector.GetSentimentPrefix(sentiment);

            // Step 2 — Check follow-up phrases
            if (IsFollowUp(input))
            {
                if (MemoryStore.LastTopic != null)
                {
                    string tip = ResponseEngine.GetTipForTopic(MemoryStore.LastTopic);
                    string topicName = ResponseEngine.GetTopicDisplayName(MemoryStore.LastTopic);
                    return $"  Here is another tip about {topicName}:\n\n{tip}" +
                           GetEncouragementSuffix(sentiment) +
                           GetFollowUpHint(MemoryStore.LastTopic);
                }
                return "  I am not sure which topic to continue on.\n" +
                       "Could you mention the topic you want more tips about?\n" +
                       "For example: 'Give me another phishing tip'";
            }

            // Step 3 — Conversational responses
            string? conversational = ResponseEngine.GetConversationalResponse(input);
            if (conversational != null)
                return sentimentPrefix + conversational;

            // Step 4 — Cybersecurity keyword detection
            string? topicKey = ResponseEngine.DetectTopic(input);
            if (topicKey != null)
            {
                MemoryStore.RecordTopicMention(topicKey);
                string? context = MemoryStore.GetInterestAcknowledgement(input, topicKey)
                                ?? MemoryStore.GetPersonalisedContext(topicKey);
                string response = ResponseEngine.GetRandomResponse(topicKey);

                return sentimentPrefix
                     + (context ?? string.Empty)
                     + response
                     + GetEncouragementSuffix(sentiment)
                     + GetFollowUpHint(topicKey);
            }

            // Step 5 — Fallback
            return sentimentPrefix + ResponseEngine.GetFallback();
        }

        private static bool IsFollowUp(string input)
        {
            string lower = input.ToLowerInvariant().Trim();
            foreach (string phrase in FollowUpPhrases)
                if (lower.Contains(phrase)) return true;
            return false;
        }

        private static string GetEncouragementSuffix(Sentiment sentiment) =>
            SentimentDetector.NeedsEncouragement(sentiment)
                ? "\n\n  Remember — knowledge is your best defence. " +
                  "You are already taking the right steps by learning about this!"
                : string.Empty;

        private static string GetFollowUpHint(string topicKey) =>
            $"\n\n  Type 'give me another tip' for more on " +
            $"{ResponseEngine.GetTopicDisplayName(topicKey)}, or ask about a different topic.";
    }
}