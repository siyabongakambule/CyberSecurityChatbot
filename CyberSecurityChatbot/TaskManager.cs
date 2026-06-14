using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace CybersecurityChatbot
{
    internal static class TaskManager
    {
        // ── In-memory task list ─
        private static readonly List<TaskItem> Tasks = new();
        private static int _nextId = 1;

        // ── MySQL connection string - 
        // Update the values to match MySQL server setup
        private const string ConnectionString =
            "Server=localhost;" +
            "Database=cybersecurity_chatbot;" +
            "Uid=root;" +
            "Pwd=;";

        // ─────────────────────────────────────────────────────────────────────────
        //  Database setup
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Creates the tasks table in MySQL 
        /// Call this once when the application starts
        /// </summary>
        internal static void InitialiseDatabase()
        {
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();

                string sql =
                    "CREATE TABLE IF NOT EXISTS tasks (" +
                    "id INT AUTO_INCREMENT PRIMARY KEY," +
                    "title VARCHAR(200) NOT NULL," +
                    "description VARCHAR(500)," +
                    "is_completed TINYINT(1) DEFAULT 0," +
                    "reminder_date DATETIME NULL," +
                    "created_at DATETIME DEFAULT CURRENT_TIMESTAMP" +
                    ");";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                // Load existing tasks from DB into memory
                LoadTasksFromDatabase(conn);

                ActivityLog.Add("Database initialised — tasks table ready.");
            }
            catch (Exception ex)
            {
                ActivityLog.Add($"Database connection failed: {ex.Message} — using in-memory storage.");
            }
        }

        private static void LoadTasksFromDatabase(MySqlConnection conn)
        {
            Tasks.Clear();
            string sql = "SELECT id, title, description, is_completed, reminder_date FROM tasks ORDER BY id;";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var task = new TaskItem
                {
                    Id = reader.GetInt32("id"),
                    Title = reader.GetString("title"),
                    Description = reader.IsDBNull(reader.GetOrdinal("description")) ? "" : reader.GetString("description"),
                    IsCompleted = reader.GetBoolean("is_completed"),
                    ReminderDate = reader.IsDBNull(reader.GetOrdinal("reminder_date"))
                        ? null
                        : reader.GetDateTime("reminder_date")
                };
                Tasks.Add(task);
                if (task.Id >= _nextId) _nextId = task.Id + 1;
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  Task operations
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>Adds a new task and saves it to the database.</summary>
        internal static string AddTask(string title, string description, DateTime? reminderDate = null)
        {
            var task = new TaskItem
            {
                Id = _nextId++,
                Title = title,
                Description = description,
                IsCompleted = false,
                ReminderDate = reminderDate
            };

            Tasks.Add(task);

            // Save to database
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();
                string sql =
                    "INSERT INTO tasks (title, description, is_completed, reminder_date) " +
                    "VALUES (@title, @desc, 0, @reminder);";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@desc", description);
                cmd.Parameters.AddWithValue("@reminder", reminderDate.HasValue ? reminderDate.Value : DBNull.Value);
                cmd.ExecuteNonQuery();
                task.Id = (int)cmd.LastInsertedId;
            }
            catch { /* use in-memory if DB unavailable */ }

            string reminderText = reminderDate.HasValue
                ? $" Reminder set for {reminderDate.Value:dd MMM yyyy}."
                : "No reminder set.";

            ActivityLog.Add($"Task added: '{title}'. {reminderText}");

            return $"  Task added successfully!\n\n" +
                   $"    Title:       {title}\n" +
                   $"    Description: {description}\n" +
                   $"   {reminderText}\n\n" +
                   $"Type 'view tasks' to see all your tasks.";
        }

        /// <summary>Returns all tasks formatted for display.</summary>
        internal static string GetAllTasks()
        {
            if (Tasks.Count == 0)
                return "  You have no tasks yet.\n\n" +
                       "Try saying:\n" +
                       "   'Add task — Enable two-factor authentication'\n" +
                       "   'Add task — Review privacy settings'";

            var sb = new StringBuilder();
            sb.AppendLine("  YOUR CYBERSECURITY TASKS");
            sb.AppendLine(new string('─', 45));

            foreach (var task in Tasks)
                sb.AppendLine(task.ToDisplayString() + "\n");

            sb.AppendLine(new string('─', 45));
            sb.AppendLine($"Total: {Tasks.Count} task(s)  |  " +
                          $"Completed: {Tasks.FindAll(t => t.IsCompleted).Count}  |  " +
                          $"Pending: {Tasks.FindAll(t => !t.IsCompleted).Count}");

            ActivityLog.Add("User viewed task list.");
            return sb.ToString();
        }

        /// <summary>Marks a task as completed by ID.</summary>
        internal static string CompleteTask(int id)
        {
            var task = Tasks.Find(t => t.Id == id);
            if (task == null)
                return $"  No task found with ID {id}. Type 'view tasks' to see your tasks.";

            task.IsCompleted = true;

            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();
                string sql = "UPDATE tasks SET is_completed = 1 WHERE id = @id;";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            catch { }

            ActivityLog.Add($"Task completed: '{task.Title}'.");
            return $"  Great work! Task '{task.Title}' marked as completed!";
        }

        /// <summary>Deletes a task by ID.</summary>
        internal static string DeleteTask(int id)
        {
            var task = Tasks.Find(t => t.Id == id);
            if (task == null)
                return $"  No task found with ID {id}. Type 'view tasks' to see your tasks.";

            Tasks.Remove(task);

            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();
                string sql = "DELETE FROM tasks WHERE id = @id;";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            catch { }

            ActivityLog.Add($"Task deleted: '{task.Title}'.");
            return $"  Task '{task.Title}' has been deleted.";
        }

        /// <summary>Sets a reminder for an existing task by ID.</summary>
        internal static string SetReminder(int id, DateTime reminderDate)
        {
            var task = Tasks.Find(t => t.Id == id);
            if (task == null)
                return $"  No task found with ID {id}.";

            task.ReminderDate = reminderDate;

            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();
                string sql = "UPDATE tasks SET reminder_date = @reminder WHERE id = @id;";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@reminder", reminderDate);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            catch { }

            ActivityLog.Add($"Reminder set for task '{task.Title}' on {reminderDate:dd MMM yyyy}.");
            return $"  Got it! Reminder set for '{task.Title}' on {reminderDate:dd MMM yyyy}.";
        }
    }
}