using System.Windows;
using System.Windows.Input;

namespace CybersecurityChatbot
{
    public partial class NameDialog : Window
    {
        /// <summary>The validated name entered by the user.</summary>
        public string EnteredName { get; private set; } = string.Empty;

        public NameDialog()
        {
            InitializeComponent();
            Loaded += (s, e) => NameBox.Focus();
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e) => TryStart();

        private void NameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                TryStart();
        }

        private void TryStart()
        {
            string name = NameBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                ErrorText.Text = "  Name cannot be empty. Please enter your name.";
                ErrorText.Visibility = Visibility.Visible;
                NameBox.Focus();
                return;
            }

            if (name.Length > 50)
            {
                ErrorText.Text = "  Name is too long. Please use a shorter name (max 50 characters).";
                ErrorText.Visibility = Visibility.Visible;
                NameBox.Focus();
                return;
            }

            EnteredName = name;
            DialogResult = true;
            Close();
        }
    }
}
