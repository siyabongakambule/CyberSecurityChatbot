using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CybersecurityChatbot
{
    public partial class MainWindow : Window
    {
        // ── Colours for chat bubbles ──────────────────────────────
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

        // ─────────────────────────────────────────────────────────
        //  Startup
        // ─────────────────────────────────────────────────────────

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Play voice greeting
            VoiceGreeting.Play();

            // Show name entry dialog
            NameDialog dialog = new NameDialog();
            dialog.Owner = this;

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

        // ─────────────────────────────────────────────────────────
        //  Button events
        // ─────────────────────────────────────────────────────────

        private void SendButton_Click(object sender, RoutedEventArgs e) => SendMessage();

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SendMessage();
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

        // ─────────────────────────────────────────────────────────
        //  Send message
        // ─────────────────────────────────────────────────────────

        private void SendMessage()
        {
            string input = InputBox.Text.Trim();
            InputBox.Clear();

            if (string.IsNullOrWhiteSpace(input)) return;

            AddUserBubble(input);

            string response = ConversationEngine.ProcessInput(input);

            AddBotBubble(response);

            UpdateMemoryBar();

            ChatScrollViewer.ScrollToBottom();
            InputBox.Focus();
        }

        // ─────────────────────────────────────────────────────────
        //  Welcome message
        // ─────────────────────────────────────────────────────────

        private void ShowWelcomeMessage()
        {
            AddDivider("═");
            AddBotBubble(
                $"  Welcome, {MemoryStore.UserName}! Great to have you here.\n\n" +
                $"I am your Cybersecurity Awareness Assistant — here to help\n" +
                $"keep YOU safe online in South Africa. 🇿🇦\n\n" +
                $"  Topics I can help you with:\n" +
                $"     Phishing and email scams\n" +
                $"   Password safety\n" +
                $"     Privacy and POPIA rights\n" +
                $"     Online scams\n" +
                $"     Malware and ransomware\n" +
                $"     Safe browsing habits\n" +
                $"     Two-factor authentication\n\n" +
                $"  You can also say:\n" +
                $"   'I am worried about scams'\n" +
                $"   'I am interested in privacy'\n" +
                $"   'Give me another tip'\n\n" +
                $"Use the quick buttons above or just type below!");
            AddDivider("─");
        }

        // ─────────────────────────────────────────────────────────
        //  Chat bubble builders
        // ─────────────────────────────────────────────────────────

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

            var label = new TextBlock
            {
                Text = "  CyberBot",
                Foreground = _borderCyan,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 9,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 4)
            };

            var text = new TextBlock
            {
                Text = message,
                Foreground = _botFg,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 11,
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 18
            };

            stack.Children.Add(label);
            stack.Children.Add(text);
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

            var label = new TextBlock
            {
                Text = $"👤  {MemoryStore.UserName}",
                Foreground = _borderGreen,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 9,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 4),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var text = new TextBlock
            {
                Text = message,
                Foreground = _userFg,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 11,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            stack.Children.Add(label);
            stack.Children.Add(text);
            container.Child = stack;
            ChatPanel.Children.Add(container);
        }

        private void AddSystemMessage(string message)
        {
            var text = new TextBlock
            {
                Text = "  " + message,
                Foreground = _systemFg,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 10,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(8, 4, 8, 4),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            ChatPanel.Children.Add(text);
        }

        private void AddDivider(string ch)
        {
            var line = new TextBlock
            {
                Text = new string(ch[0], 80),
                Foreground = _divider,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 8,
                Margin = new Thickness(4, 2, 4, 2)
            };
            ChatPanel.Children.Add(line);
        }

        // ─────────────────────────────────────────────────────────
        //  Memory bar
        // ─────────────────────────────────────────────────────────

        private void UpdateMemoryBar()
        {
            string favourite = MemoryStore.FavouriteTopic != null
                ? ResponseEngine.GetTopicDisplayName(MemoryStore.FavouriteTopic)
                : "None";

            string last = MemoryStore.LastTopic != null
                ? ResponseEngine.GetTopicDisplayName(MemoryStore.LastTopic)
                : "None";

            MemoryBar.Text =
                $"  Memory — Name: {MemoryStore.UserName}  |  " +
                $"Favourite topic: {favourite}  |  " +
                $"Last topic: {last}";
        }
    }
}
