using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace CybersecurityChatbot
{
    /// <summary>Describes the outcome of validating user input.</summary>
    internal enum ValidationStatus
    {
        Valid,
        Empty,
        TooLong,
        NumbersOnly,
        SymbolsOnly
    }

    internal static class InputValidator
    {
        private const int MaxInputLength = 300;

        /// <summary>
        /// Validates raw console input and returns a status code
        /// plus a human-friendly error message when invalid.
        /// </summary>
        internal static (ValidationStatus Status, string? ErrorMessage) Validate(string? raw)
        {
            // ── Empty / whitespace ────────────────────────────────────────────────
            if (string.IsNullOrWhiteSpace(raw))
            {
                return (ValidationStatus.Empty,
                    "Hmm, it looks like you didn't type anything.\n" +
                    "    Feel free to ask a question or type 'help' to see available topics.");
            }

            string trimmed = raw.Trim();

            // ── Too long ──────────────────────────────────────────────────────────
            if (trimmed.Length > MaxInputLength)
            {
                return (ValidationStatus.TooLong,
                    $"That message is a bit too long (max {MaxInputLength} characters).\n" +
                    "    Please shorten your question and try again.");
            }

            // ── Numbers only (not useful input) ───────────────────────────────────
            if (IsNumericOnly(trimmed))
            {
                return (ValidationStatus.NumbersOnly,
                    "I received only numbers, which I can't quite make sense of.\n" +
                    "    Try asking a question using words — for example: 'What is phishing?'");
            }

            // ── Symbols only ──────────────────────────────────────────────────────
            if (IsSymbolsOnly(trimmed))
            {
                return (ValidationStatus.SymbolsOnly,
                    "That looks like symbols only — I didn't quite catch what you meant.\n" +
                    "    Could you rephrase using words? Type 'help' for available topics.");
            }

            return (ValidationStatus.Valid, null);
        }

        // ─────────────────────────────────────────────────────────────
        //  Helpers
        // ─────────────────────────────────────────────────────────────

        private static bool IsNumericOnly(string input)
        {
            foreach (char c in input)
                if (!char.IsDigit(c) && c != '.' && c != ',' && c != ' ')
                    return false;
            return true;
        }

        private static bool IsSymbolsOnly(string input)
        {
            foreach (char c in input)
                if (char.IsLetter(c) || char.IsDigit(c))
                    return false;
            return true;
        }
    }
}
