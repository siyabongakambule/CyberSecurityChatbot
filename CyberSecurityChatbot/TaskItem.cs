using System;

namespace CybersecurityChatbot
{
    internal class TaskItem
    {
        internal int Id { get; set; }
        internal string Title { get; set; } = string.Empty;
        internal string Description { get; set; } = string.Empty;
        internal bool IsCompleted { get; set; } = false;
        internal DateTime? ReminderDate { get; set; }

        /// <summary>Returns a formatted display string for this task.</summary>
        internal string ToDisplayString()
        {
            string status = IsCompleted ? "yes" : "time";
            string reminder = ReminderDate.HasValue
                ? $"   Reminder: {ReminderDate.Value:dd MMM yyyy}"
                : "  No reminder set";
            return $"{status}  [{Id}] {Title}\n       {Description}\n       {reminder}";
        }
    }
}
