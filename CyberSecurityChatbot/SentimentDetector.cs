using System.Collections.Generic;

namespace CybersecurityChatbot
{
    internal enum Sentiment { Neutral, Worried, Frustrated, Curious, Confused, Positive }

    internal static class SentimentDetector
    {
        private static readonly List<string> WorriedWords = new() { "worried", "scared", "afraid", "anxious", "nervous", "fear", "unsafe", "vulnerable", "panic", "concerned", "help me", "dangerous" };
        private static readonly List<string> FrustratedWords = new() { "frustrated", "annoyed", "angry", "dont understand", "don't understand", "so confusing", "hate this", "useless", "fed up", "sick of", "ridiculous" };
        private static readonly List<string> CuriousWords = new() { "curious", "interesting", "tell me more", "how does", "what is", "explain", "i want to know", "wondering", "give me a tip", "another tip", "learn more" };
        private static readonly List<string> ConfusedWords = new() { "confused", "confusing", "dont get it", "don't get it", "not sure", "unclear", "lost", "what do you mean", "can you clarify", "explain more" };
        private static readonly List<string> PositiveWords = new() { "great", "awesome", "thanks", "thank you", "helpful", "love this", "good", "amazing", "excellent", "perfect", "brilliant", "appreciate" };

        internal static Sentiment Detect(string input)
        {
            string lower = input.ToLowerInvariant();
            foreach (string kw in WorriedWords) if (lower.Contains(kw)) return Sentiment.Worried;
            foreach (string kw in FrustratedWords) if (lower.Contains(kw)) return Sentiment.Frustrated;
            foreach (string kw in ConfusedWords) if (lower.Contains(kw)) return Sentiment.Confused;
            foreach (string kw in CuriousWords) if (lower.Contains(kw)) return Sentiment.Curious;
            foreach (string kw in PositiveWords) if (lower.Contains(kw)) return Sentiment.Positive;
            return Sentiment.Neutral;
        }

        internal static string GetSentimentPrefix(Sentiment sentiment) => sentiment switch
        {
            Sentiment.Worried =>
                "  It is completely understandable to feel that way — " +
                "cyber threats can be very overwhelming. " +
                "Let me share some guidance to help you stay safe:\n\n",
            Sentiment.Frustrated =>
                "  I hear you — cybersecurity can feel very complicated at times. " +
                "Let me try to explain this as clearly as possible:\n\n",
            Sentiment.Curious =>
                "  Great question! Learning about cybersecurity is the best way " +
                "to stay protected. Here is what you need to know:\n\n",
            Sentiment.Confused =>
                "  No worries at all — let me explain this more clearly for you:\n\n",
            Sentiment.Positive =>
                "  Glad to hear that! Here is some more useful information:\n\n",
            _ => string.Empty
        };

        internal static bool NeedsEncouragement(Sentiment sentiment) =>
            sentiment == Sentiment.Worried || sentiment == Sentiment.Frustrated;
    }
}
