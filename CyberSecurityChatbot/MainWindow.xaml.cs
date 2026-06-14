using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CybersecurityChatbot
{
    public partial class MainWindow : Window
    {
        // ── Colours ───────────────────────────────────────────────────────────────
        private readonly SolidColorBrush _botBg = new(Color.FromRgb(10, 32, 64));
        private readonly SolidColorBrush _userBg = new(Color.FromRgb(10, 48, 32));
        private readonly SolidColorBrush _botFg = new(Color.FromRgb(0, 220, 180));
        private readonly SolidColorBrush _userFg = new(Color.FromRgb(255, 215, 64));
        private readonly SolidColorBrush _systemFg = new(Color.FromRgb(160, 180, 255));
        private readonly SolidColorBrush _borderCyan = new(Color.FromRgb(0, 180, 180));
        private readonly SolidColorBrush _borderGreen = new(Color.FromRgb(0, 180, 100));
        private readonly SolidColorBrush _divider = new(Color.FromRgb(20, 40, 80));

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  Startup
        // ─────────────────────────────────────────────────────────────────────────

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialise database
            TaskManager.InitialiseDatabase();

            // Play voice greeting
            VoiceGreeting.Play();

            // Show name dialog
            NameDialog dialog = new NameDialog { Owner = this };
            bool? result = dialog.ShowDialog();

            if (result == true && !string.IsNullOrWhiteSpace(dialog.EnteredName))
            {
                MemoryStore.UserName = dialog.EnteredName.Trim();
                ShowWelcomeMessage();
                UpdateMemoryBar();
                InputBox.Focus();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  Chat tab events
        // ─────────────────────────────────────────────────────────────────────────

        private void SendButton_Click(object sender, RoutedEventArgs e) => SendMessage();

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SendMessage();
        }

        private void TopicButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string query)
            {
                InputBox.Text = query;
                SendMessage();
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ChatPanel.Children.Clear();
            AddSystemMessage("Chat cleared. How can I help you, " + MemoryStore.UserName + "?");
        }

        private void SendMessage()
        {
            string input = InputBox.Text.Trim();
            InputBox.Clear();
            if (string.IsNullOrWhiteSpace(input)) return;

            AddUserBubble(input);
            string response = ConversationEngine.ProcessInput(input);
            AddBotBubble(response);
            UpdateMemoryBar();
            RefreshActivityLog();
            ChatScrollViewer.ScrollToBottom();
            InputBox.Focus();
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  Task tab events
        // ─────────────────────────────────────────────────────────────────────────

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TaskTitleBox.Text.Trim();
            string desc = TaskDescBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                TaskDisplay.Text = "  Please enter a task title.";
                return;
            }

            if (string.IsNullOrWhiteSpace(desc))
                desc = GenerateDescription(title);

            DateTime? reminder = ReminderDatePicker.SelectedDate;
            string result = TaskManager.AddTask(title, desc, reminder);
            TaskDisplay.Text = result;
            TaskTitleBox.Clear();
            TaskDescBox.Clear();
            ReminderDatePicker.SelectedDate = null;
            RefreshActivityLog();
        }

        private void ViewTasksButton_Click(object sender, RoutedEventArgs e)
        {
            TaskDisplay.Text = TaskManager.GetAllTasks();
            RefreshActivityLog();
        }

        private void CompleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TaskIdBox.Text.Trim(), out int id))
            {
                TaskDisplay.Text = TaskManager.CompleteTask(id);
                RefreshActivityLog();
            }
            else
            {
                TaskDisplay.Text = "  Please enter a valid Task ID in the box next to the buttons.";
            }
        }

        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TaskIdBox.Text.Trim(), out int id))
            {
                TaskDisplay.Text = TaskManager.DeleteTask(id);
                RefreshActivityLog();
            }
            else
            {
                TaskDisplay.Text = "  Please enter a valid Task ID in the box next to the buttons.";
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  Quiz tab events
        // ─────────────────────────────────────────────────────────────────────────

        private void StartQuizButton_Click(object sender, RoutedEventArgs e)
        {
            QuizDisplay.Text = QuizEngine.StartQuiz();
            QuizAnswerBox.Focus();
        }

        private void SubmitAnswerButton_Click(object sender, RoutedEventArgs e) => SubmitQuizAnswer();

        private void QuizAnswerBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SubmitQuizAnswer();
        }

        private void SubmitQuizAnswer()
        {
            string answer = QuizAnswerBox.Text.Trim();
            QuizAnswerBox.Clear();
            if (string.IsNullOrWhiteSpace(answer)) return;

            QuizDisplay.Text = QuizEngine.ProcessAnswer(answer);
            RefreshActivityLog();
            QuizAnswerBox.Focus();
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  Activity log tab events
        // ─────────────────────────────────────────────────────────────────────────

        private void RefreshLogButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshActivityLog();
        }

        private void RefreshActivityLog()
        {
            LogDisplay.Text = ActivityLog.GetLog();
            LogScrollViewer.ScrollToBottom();
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  Welcome message
        // ─────────────────────────────────────────────────────────────────────────

        private void ShowWelcomeMessage()
        {
            AddDivider("═");
            AddBotBubble(
                $"  Welcome, {MemoryStore.UserName}! Great to have you here.\n\n" +
                $"I am your Cybersecurity Awareness Assistant \n\n" +
                $"     Task Assistant — manage cybersecurity tasks\n" +
                $"     Mini Quiz — test your cybersecurity knowledge\n" +
                $"     NLP — understand natural language requests\n" +
                $"     Activity Log — track all bot actions\n\n" +
                $"    Phishing   Passwords   Privacy\n" +
                $"    Malware   Browsing   2FA\n\n" +
                $"Try saying:\n" +
                $"   'Add task — Enable two-factor authentication'\n" +
                $"   'Remind me to update my password in 3 days'\n" +
                $"   'Start quiz'\n" +
                $"   'Show activity log'\n\n" +
                $"Use the tabs above or quick buttons to get started!");
            AddDivider("─");
            ActivityLog.Add($"Session started — User: {MemoryStore.UserName}.");
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  Chat bubble builders
        // ─────────────────────────────────────────────────────────────────────────

        private void AddBotBubble(string message)
        {
            var container = new Border
            {
                Background = _botBg,
                BorderBrush = _borderCyan,
                BorderThickness = new Thickness(2, 0, 0, 0),
                CornerRadius = new CornerRadius(0, 8, 8, 0),
                Margin = new Thickness(4, 4, 60, 4),
                Padding = new Thickness(14, 10, 14, 10)
            };
            var stack = new StackPanel();
            stack.Children.Add(new TextBlock
            {
                Text = "  CyberBot",
                Foreground = _borderCyan,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 9,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 4)
            });
            stack.Children.Add(new TextBlock
            {
                Text = message,
                Foreground = _botFg,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 11,
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 18
            });
            container.Child = stack;
            ChatPanel.Children.Add(container);
        }

        private void AddUserBubble(string message)
        {
            var container = new Border
            {
                Background = _userBg,
                BorderBrush = _borderGreen,
                BorderThickness = new Thickness(0, 0, 2, 0),
                CornerRadius = new CornerRadius(8, 0, 0, 8),
                Margin = new Thickness(60, 4, 4, 4),
                Padding = new Thickness(14, 10, 14, 10),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            var stack = new StackPanel();
            stack.Children.Add(new TextBlock
            {
                Text = $"  {MemoryStore.UserName}",
                Foreground = _borderGreen,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 9,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 4),
                HorizontalAlignment = HorizontalAlignment.Right
            });
            stack.Children.Add(new TextBlock
            {
                Text = message,
                Foreground = _userFg,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 11,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Right
            });
            container.Child = stack;
            ChatPanel.Children.Add(container);
        }

        private void AddSystemMessage(string message)
        {
            ChatPanel.Children.Add(new TextBlock
            {
                Text = "  " + message,
                Foreground = _systemFg,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 10,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(8, 4, 8, 4),
                HorizontalAlignment = HorizontalAlignment.Center
            });
        }

        private void AddDivider(string ch)
        {
            ChatPanel.Children.Add(new TextBlock
            {
                Text = new string(ch[0], 80),
                Foreground = _divider,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 8,
                Margin = new Thickness(4, 2, 4, 2)
            });
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  Memory bar and helpers
        // ─────────────────────────────────────────────────────────────────────────

        private void UpdateMemoryBar()
        {
            string favourite = MemoryStore.FavouriteTopic != null
                ? ResponseEngine.GetTopicDisplayName(MemoryStore.FavouriteTopic) : "None";
            string last = MemoryStore.LastTopic != null
                ? ResponseEngine.GetTopicDisplayName(MemoryStore.LastTopic) : "None";
            MemoryBar.Text =
                $"  Memory — Name: {MemoryStore.UserName}  |  " +
                $"Favourite: {favourite}  |  Last topic: {last}  |  " +
                $"Log entries: {ActivityLog.Count}";
        }

        private static string GenerateDescription(string title)
        {
            string lower = title.ToLower();
            if (lower.Contains("two-factor") || lower.Contains("2fa"))
                return "Enable two-factor authentication to add an extra security layer.";
            if (lower.Contains("password"))
                return "Update and strengthen passwords using a password manager.";
            if (lower.Contains("privacy"))
                return "Review privacy settings to protect your personal data.";
            if (lower.Contains("backup"))
                return "Back up important files using the 3-2-1 backup rule.";
            return $"Complete cybersecurity task: {title}.";
        }
    }
}
