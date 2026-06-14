namespace CybersecurityChatbot
{
    internal static class ConversationEngine
    {
        private static readonly string[] FollowUpPhrases =
        {
            "give me another tip", "another tip", "tell me more", "explain more",
            "more details", "more info", "continue", "go on", "keep going",
            "elaborate", "more please", "what else", "another one", "give me more"
        };

        internal static string ProcessInput(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return "  Please type something — I am here to help!";

            string input = userInput.Trim();

            // ── Step 1: If quiz is active route answers to quiz engine ────────────
            if (QuizEngine.IsActive)
            {
                return QuizEngine.ProcessAnswer(input);
            }

            // ── Step 2: Detect sentiment ──────────────────────────────────────────
            Sentiment sentiment = SentimentDetector.Detect(input);
            string sentimentPrefix = SentimentDetector.GetSentimentPrefix(sentiment);

            // ── Step 3: Run NLP analysis ──────────────────────────────────────────
            NlpResult nlp = NlpProcessor.Analyse(input);

            switch (nlp.Intent)
            {
                // ── Quiz ──────────────────────────────────────────────────────────
                case UserIntent.StartQuiz:
                    ActivityLog.Add("User started the cybersecurity quiz.");
                    return QuizEngine.StartQuiz();

                // ── Activity log ──────────────────────────────────────────────────
                case UserIntent.ViewActivityLog:
                    return ActivityLog.GetLog();

                // ── View tasks ────────────────────────────────────────────────────
                case UserIntent.ViewTasks:
                    return TaskManager.GetAllTasks();

                // ── Add task ──────────────────────────────────────────────────────
                case UserIntent.AddTask:
                    string title = !string.IsNullOrWhiteSpace(nlp.TaskTitle)
                        ? nlp.TaskTitle
                        : ExtractFallbackTitle(input);

                    string desc = GenerateTaskDescription(title);
                    return TaskManager.AddTask(title, desc, nlp.ReminderDate);

                // ── Complete task ─────────────────────────────────────────────────
                case UserIntent.CompleteTask:
                    if (nlp.TaskId > 0)
                        return TaskManager.CompleteTask(nlp.TaskId);
                    return "  Please specify a task ID. Example: 'complete task 1'";

                // ── Delete task ───────────────────────────────────────────────────
                case UserIntent.DeleteTask:
                    if (nlp.TaskId > 0)
                        return TaskManager.DeleteTask(nlp.TaskId);
                    return "  Please specify a task ID. Example: 'delete task 1'";

                // ── Cybersecurity topic ───────────────────────────────────────────
                case UserIntent.CybersecurityTopic:
                    if (nlp.TopicKey != null)
                    {
                        MemoryStore.RecordTopicMention(nlp.TopicKey);
                        string? context = MemoryStore.GetInterestAcknowledgement(input, nlp.TopicKey)
                                        ?? MemoryStore.GetPersonalisedContext(nlp.TopicKey);
                        string response = ResponseEngine.GetRandomResponse(nlp.TopicKey);
                        ActivityLog.Add($"Cybersecurity topic discussed: {ResponseEngine.GetTopicDisplayName(nlp.TopicKey)}.");
                        return sentimentPrefix
                             + (context ?? string.Empty)
                             + response
                             + GetEncouragementSuffix(sentiment)
                             + GetFollowUpHint(nlp.TopicKey);
                    }
                    break;

                // ── Conversational ────────────────────────────────────────────────
                case UserIntent.Conversational:
                    // Check follow-ups
                    if (IsFollowUp(input) && MemoryStore.LastTopic != null)
                    {
                        string tip = ResponseEngine.GetTipForTopic(MemoryStore.LastTopic);
                        return $"  Here is another tip about " +
                               $"{ResponseEngine.GetTopicDisplayName(MemoryStore.LastTopic)}:\n\n{tip}" +
                               GetFollowUpHint(MemoryStore.LastTopic);
                    }

                    string? convo = ResponseEngine.GetConversationalResponse(input);
                    if (convo != null)
                        return sentimentPrefix + convo;
                    break;
            }

            // ── Fallback ──────────────────────────────────────────────────────────
            ActivityLog.Add($"Unrecognised input: '{input}'.");
            return sentimentPrefix + ResponseEngine.GetFallback();
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private static bool IsFollowUp(string input)
        {
            string lower = input.ToLowerInvariant().Trim();
            foreach (string phrase in FollowUpPhrases)
                if (lower.Contains(phrase)) return true;
            return false;
        }

        private static string GetEncouragementSuffix(Sentiment sentiment) =>
            SentimentDetector.NeedsEncouragement(sentiment)
                ? "\n\n  Remember — knowledge is your best defence!"
                : string.Empty;

        private static string GetFollowUpHint(string topicKey) =>
            $"\n\n  Type 'give me another tip' for more on " +
            $"{ResponseEngine.GetTopicDisplayName(topicKey)}.";

        private static string ExtractFallbackTitle(string input)
        {
            // Remove common command words and return the rest as the title
            string lower = input.ToLower()
                .Replace("add task", "").Replace("add a task", "")
                .Replace("create task", "").Replace("new task", "")
                .Replace("remind me to", "").Replace("remind me", "")
                .Replace("-", "").Trim();
            return string.IsNullOrWhiteSpace(lower) ? "Cybersecurity task" : lower;
        }

        private static string GenerateTaskDescription(string title)
        {
            string lower = title.ToLower();
            if (lower.Contains("two-factor") || lower.Contains("2fa"))
                return "Enable two-factor authentication on all important accounts to add an extra layer of security.";
            if (lower.Contains("password"))
                return "Update and strengthen your passwords using a mix of characters and a password manager.";
            if (lower.Contains("privacy"))
                return "Review account privacy settings to ensure your personal data is protected.";
            if (lower.Contains("backup"))
                return "Back up important files using the 3-2-1 rule: 3 copies, 2 media types, 1 offsite.";
            if (lower.Contains("antivirus") || lower.Contains("update"))
                return "Keep software and antivirus updated to protect against known vulnerabilities.";
            return $"Complete the cybersecurity task: {title}.";
        }
    }
}