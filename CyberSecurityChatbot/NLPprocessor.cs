using System;
using System.Text.RegularExpressions;

namespace CybersecurityChatbot
{
    internal enum UserIntent
    {
        None,
        AddTask,
        ViewTasks,
        CompleteTask,
        DeleteTask,
        SetReminder,
        StartQuiz,
        ViewActivityLog,
        CybersecurityTopic,
        Conversational
    }

    internal class NlpResult
    {
        internal UserIntent Intent { get; set; } = UserIntent.None;
        internal string? TaskTitle { get; set; }
        internal int TaskId { get; set; } = -1;
        internal DateTime? ReminderDate { get; set; }
        internal string? TopicKey { get; set; }
    }

    internal static class NlpProcessor
    {
        // ── Regex patterns for intent detection ───────────────────────────────────

        // Add task patterns
        private static readonly Regex AddTaskPattern = new(
            @"(add|create|new|make|set up|setup)\s+(a\s+)?(task|reminder|todo|to-do|to do)\s*(to\s+|for\s+|-\s*)?(.+)",
            RegexOptions.IgnoreCase);

        // Reminder patterns — "remind me to X in Y days"
        private static readonly Regex ReminderPattern = new(
            @"remind\s+me\s+(to\s+)?(.+?)(\s+in\s+(\d+)\s+(day|days|week|weeks|month|months))?$",
            RegexOptions.IgnoreCase);

        // Days extraction — "in 3 days", "in 2 weeks"
        private static readonly Regex DaysPattern = new(
            @"in\s+(\d+)\s+(day|days|week|weeks|month|months)",
            RegexOptions.IgnoreCase);

        // Complete task — "complete task 2", "mark task 3 as done"
        private static readonly Regex CompletePattern = new(
            @"(complete|finish|done|mark|tick)\s+(task\s+)?(\d+)",
            RegexOptions.IgnoreCase);

        // Delete task
        private static readonly Regex DeletePattern = new(
            @"(delete|remove|cancel)\s+(task\s+)?(\d+)",
            RegexOptions.IgnoreCase);

        // Task ID extraction
        private static readonly Regex TaskIdPattern = new(@"\d+", RegexOptions.IgnoreCase);

        // ─────────────────────────────────────────────────────────────────────────
        //  Main NLP method
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Analyses user input and returns a structured NlpResult
        /// describing what the user wants to do.
        /// </summary>
        internal static NlpResult Analyse(string input)
        {
            string lower = input.ToLowerInvariant().Trim();
            var result = new NlpResult();

            // ── Quiz intent ───────────────────────────────────────────────────────
            if (lower.Contains("quiz") || lower.Contains("mini game") ||
                lower.Contains("minigame") || lower.Contains("test me") ||
                lower.Contains("start game") || lower.Contains("play game"))
            {
                result.Intent = UserIntent.StartQuiz;
                return result;
            }

            // ── Activity log intent ───────────────────────────────────────────────
            if (lower.Contains("activity log") || lower.Contains("show log") ||
                lower.Contains("what have you done") || lower.Contains("recent actions") ||
                lower.Contains("show history") || lower.Contains("view log"))
            {
                result.Intent = UserIntent.ViewActivityLog;
                return result;
            }

            // ── View tasks intent ─────────────────────────────────────────────────
            if ((lower.Contains("view") || lower.Contains("show") ||
                 lower.Contains("list") || lower.Contains("my tasks") ||
                 lower.Contains("see tasks")) &&
                (lower.Contains("task") || lower.Contains("reminder")))
            {
                result.Intent = UserIntent.ViewTasks;
                return result;
            }

            // ── Complete task intent ──────────────────────────────────────────────
            var completeMatch = CompletePattern.Match(input);
            if (completeMatch.Success)
            {
                result.Intent = UserIntent.CompleteTask;
                result.TaskId = int.Parse(completeMatch.Groups[3].Value);
                return result;
            }

            // ── Delete task intent ────────────────────────────────────────────────
            var deleteMatch = DeletePattern.Match(input);
            if (deleteMatch.Success)
            {
                result.Intent = UserIntent.DeleteTask;
                result.TaskId = int.Parse(deleteMatch.Groups[3].Value);
                return result;
            }

            // ── Reminder intent — "remind me to X in Y days" ─────────────────────
            var reminderMatch = ReminderPattern.Match(input);
            if (reminderMatch.Success && lower.Contains("remind"))
            {
                result.Intent = UserIntent.AddTask;
                result.TaskTitle = reminderMatch.Groups[2].Value.Trim();
                result.ReminderDate = ExtractDate(input);
                return result;
            }

            // ── Add task intent ───────────────────────────────────────────────────
            var addMatch = AddTaskPattern.Match(input);
            if (addMatch.Success)
            {
                result.Intent = UserIntent.AddTask;
                result.TaskTitle = addMatch.Groups[5].Value.Trim();
                result.ReminderDate = ExtractDate(input);
                return result;
            }

            // ── Flexible add task detection ───────────────────────────────────────
            if ((lower.Contains("add") || lower.Contains("create") || lower.Contains("new")) &&
                (lower.Contains("task") || lower.Contains("todo") || lower.Contains("reminder")))
            {
                result.Intent = UserIntent.AddTask;
                result.TaskTitle = ExtractTaskTitle(input);
                result.ReminderDate = ExtractDate(input);
                return result;
            }

            // ── Cybersecurity topic detection ─────────────────────────────────────
            string? topicKey = ResponseEngine.DetectTopic(input);
            if (topicKey != null)
            {
                result.Intent = UserIntent.CybersecurityTopic;
                result.TopicKey = topicKey;
                return result;
            }

            // ── Conversational ────────────────────────────────────────────────────
            result.Intent = UserIntent.Conversational;
            return result;
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        /// <summary>Extracts a date from phrases like "in 3 days", "in 2 weeks".</summary>
        internal static DateTime? ExtractDate(string input)
        {
            var match = DaysPattern.Match(input);
            if (!match.Success) return null;

            int number = int.Parse(match.Groups[1].Value);
            string unit = match.Groups[2].Value.ToLower();

            return unit.StartsWith("week") ? DateTime.Now.AddDays(number * 7) :
                   unit.StartsWith("month") ? DateTime.Now.AddMonths(number) :
                                              DateTime.Now.AddDays(number);
        }

        /// <summary>
        /// Tries to extract a task title from input by removing common command words.
        /// </summary>
        private static string ExtractTaskTitle(string input)
        {
            string[] removeWords = { "add", "create", "new", "a", "task", "todo",
                                     "to-do", "reminder", "to", "for", "please", "-" };
            string result = input;
            foreach (string word in removeWords)
                result = Regex.Replace(result, $@"\b{word}\b", "", RegexOptions.IgnoreCase);

            // Remove date phrases
            result = DaysPattern.Replace(result, "");
            result = Regex.Replace(result, @"\bin\b", "", RegexOptions.IgnoreCase);

            return result.Trim().Trim('-').Trim();
        }
    }
}
