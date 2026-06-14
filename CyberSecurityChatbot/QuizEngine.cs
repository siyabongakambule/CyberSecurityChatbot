using System;
using System.Collections.Generic;

namespace CybersecurityChatbot
{
    internal class QuizQuestion
    {
        internal string Question { get; set; } = string.Empty;
        internal List<string> Options { get; set; } = new();
        internal string Answer { get; set; } = string.Empty;
        internal string Explanation { get; set; } = string.Empty;
        internal bool IsTrueFalse { get; set; } = false;
    }

    internal static class QuizEngine
    {
        // ── All quiz questions ────────────────────────────────────────────────────
        private static readonly List<QuizQuestion> AllQuestions = new()
        {
            new QuizQuestion
            {
                Question    = "What should you do if you receive an email asking for your password?",
                Options     = new() { "A) Reply with your password", "B) Delete the email", "C) Report it as phishing", "D) Ignore it" },
                Answer      = "C",
                Explanation = "  Correct! Reporting phishing emails helps protect you and others from scams. Legitimate organisations never ask for passwords via email."
            },
            new QuizQuestion
            {
                Question    = "True or False: Using the same password on multiple websites is safe as long as it is strong.",
                Options     = new() { "A) True", "B) False" },
                Answer      = "B",
                Explanation = "  False! If one website is breached, all your other accounts become vulnerable. Always use unique passwords for each site.",
                IsTrueFalse = true
            },
            new QuizQuestion
            {
                Question    = "What does HTTPS in a website URL indicate?",
                Options     = new() { "A) The website is fast", "B) The connection is encrypted and secure", "C) The website is government owned", "D) The website is free" },
                Answer      = "B",
                Explanation = "  Correct! HTTPS means the connection between your browser and the website is encrypted, protecting your data in transit."
            },
            new QuizQuestion
            {
                Question    = "What is two-factor authentication (2FA)?",
                Options     = new() { "A) Using two different passwords", "B) Logging in from two devices", "C) A second verification step beyond your password", "D) Having two email accounts" },
                Answer      = "C",
                Explanation = "  Correct! 2FA adds an extra layer of security by requiring a second form of verification such as an OTP or authenticator app."
            },
            new QuizQuestion
            {
                Question    = "True or False: Public Wi-Fi is safe to use for online banking.",
                Options     = new() { "A) True", "B) False" },
                Answer      = "B",
                Explanation = "  False! Public Wi-Fi is often unsecured and attackers can intercept your data. Always use a VPN or mobile data for banking.",
                IsTrueFalse = true
            },
            new QuizQuestion
            {
                Question    = "Which of these is the strongest password?",
                Options     = new() { "A) password123", "B) siya2004", "C) Coffee!Rain@Joburg2025", "D) 12345678" },
                Answer      = "C",
                Explanation = "  Correct! A strong password is long, uses mixed character types, and avoids personal information."
            },
            new QuizQuestion
            {
                Question    = "What is phishing?",
                Options     = new() { "A) A type of malware that encrypts files", "B) A fraudulent attempt to steal information by disguising as a trusted source", "C) A secure email protocol", "D) A type of firewall" },
                Answer      = "B",
                Explanation = "  Correct! Phishing tricks users into revealing sensitive information by pretending to be a legitimate organisation."
            },
            new QuizQuestion
            {
                Question    = "True or False: You should always plug in USB drives you find in public places to check their contents.",
                Options     = new() { "A) True", "B) False" },
                Answer      = "B",
                Explanation = "  False! Unknown USB drives are a common malware delivery method called baiting. Never plug in a USB you did not purchase yourself.",
                IsTrueFalse = true
            },
            new QuizQuestion
            {
                Question    = "What does ransomware do?",
                Options     = new() { "A) Speeds up your computer", "B) Steals your passwords silently", "C) Encrypts your files and demands payment", "D) Monitors your browsing habits" },
                Answer      = "C",
                Explanation = "  Correct! Ransomware encrypts your files and demands a ransom payment for the decryption key. Regular backups are your best defence."
            },
            new QuizQuestion
            {
                Question    = "What is social engineering?",
                Options     = new() { "A) Building social media apps", "B) Manipulating people into revealing confidential information", "C) Engineering software for social networks", "D) A type of antivirus software" },
                Answer      = "B",
                Explanation = "  Correct! Social engineering exploits human psychology rather than technical vulnerabilities to gain access to systems or information."
            },
            new QuizQuestion
            {
                Question    = "True or False: SARS will ask for your banking details via WhatsApp.",
                Options     = new() { "A) True", "B) False" },
                Answer      = "B",
                Explanation = "  False! SARS will NEVER ask for banking details via WhatsApp, SMS, or email. Any such message is a scam.",
                IsTrueFalse = true
            },
            new QuizQuestion
            {
                Question    = "Which is the safest way to store your passwords?",
                Options     = new() { "A) Write them on a sticky note", "B) Use the same password for everything", "C) Use a reputable password manager", "D) Save them in a text file on your desktop" },
                Answer      = "C",
                Explanation = "  Correct! A reputable password manager like Bitwarden securely stores and generates strong unique passwords for all your accounts."
            }
        };

        // ── State ─────────────────────────────────────────────────────────────────
        private static List<QuizQuestion> _currentQuiz = new();
        private static int _currentIndex = 0;
        private static int _score = 0;
        private static bool _isActive = false;
        private static readonly Random Rng = new();

        // ── Public interface ──────────────────────────────────────────────────────

        internal static bool IsActive => _isActive;

        /// <summary>Starts a new quiz session.</summary>
        internal static string StartQuiz()
        {
            // Shuffle and pick 10 questions
            _currentQuiz = ShuffleQuestions();
            _currentIndex = 0;
            _score = 0;
            _isActive = true;

            ActivityLog.Add("Quiz started.");
            return "  CYBERSECURITY QUIZ STARTED!\n\n" +
                   $"You will be asked {_currentQuiz.Count} questions.\n" +
                   "Type the letter of your answer (A, B, C, or D).\n" +
                   new string('─', 45) + "\n\n" +
                   GetCurrentQuestion();
        }

        /// <summary>Processes the user's answer and returns feedback.</summary>
        internal static string ProcessAnswer(string input)
        {
            if (!_isActive)
                return "No quiz is active. Type 'start quiz' to begin!";

            string answer = input.Trim().ToUpperInvariant();

            // Extract just the letter if user types "A)" or "A."
            if (answer.Length > 1 && (answer[1] == ')' || answer[1] == '.'))
                answer = answer[0].ToString();

            var question = _currentQuiz[_currentIndex];
            bool isCorrect = answer == question.Answer.ToUpperInvariant();

            string feedback;
            if (isCorrect)
            {
                _score++;
                feedback = $"  CORRECT!\n{question.Explanation}";
                ActivityLog.Add($"Quiz Q{_currentIndex + 1}: Correct answer.");
            }
            else
            {
                feedback = $"  INCORRECT! The correct answer was {question.Answer}.\n{question.Explanation}";
                ActivityLog.Add($"Quiz Q{_currentIndex + 1}: Incorrect answer.");
            }

            _currentIndex++;

            if (_currentIndex >= _currentQuiz.Count)
            {
                _isActive = false;
                return feedback + "\n\n" + GetFinalScore();
            }

            return feedback + "\n\n" +
                   new string('─', 45) + "\n\n" +
                   GetCurrentQuestion();
        }

        /// <summary>Returns the current question formatted for display.</summary>
        internal static string GetCurrentQuestion()
        {
            if (_currentIndex >= _currentQuiz.Count) return string.Empty;

            var q = _currentQuiz[_currentIndex];
            var sb = new System.Text.StringBuilder();

            sb.AppendLine($"?  Question {_currentIndex + 1} of {_currentQuiz.Count}:");
            sb.AppendLine();
            sb.AppendLine($"   {q.Question}");
            sb.AppendLine();

            foreach (string option in q.Options)
                sb.AppendLine($"   {option}");

            sb.AppendLine();
            sb.Append("Your answer: ");
            return sb.ToString();
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private static string GetFinalScore()
        {
            int total = _currentQuiz.Count;
            int percentage = (_score * 100) / total;

            string grade = percentage switch
            {
                >= 90 => "  Outstanding! You are a Cybersecurity Pro!",
                >= 70 => "  Great job! You have solid cybersecurity knowledge!",
                >= 50 => "  Good effort! Keep learning to stay safe online.",
                _ => "  Keep studying! Cybersecurity knowledge is your best defence."
            };

            ActivityLog.Add($"Quiz completed — Score: {_score}/{total} ({percentage}%).");

            return $"  QUIZ COMPLETE!\n\n" +
                   $"   Your score: {_score} out of {total} ({percentage}%)\n\n" +
                   $"   {grade}\n\n" +
                   "Type 'start quiz' to try again or ask about any cybersecurity topic!";
        }

        private static List<QuizQuestion> ShuffleQuestions()
        {
            var shuffled = new List<QuizQuestion>(AllQuestions);
            for (int i = shuffled.Count - 1; i > 0; i--)
            {
                int j = Rng.Next(i + 1);
                (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
            }
            return shuffled.GetRange(0, Math.Min(10, shuffled.Count));
        }
    }
}
