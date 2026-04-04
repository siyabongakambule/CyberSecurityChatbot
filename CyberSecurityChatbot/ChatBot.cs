using CybersecurityChatbot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CybersecurityChatbot
{
    internal static class ChatBot
    {
        /// <summary>
        /// Enters the main conversation loop and keeps running until
        /// the user types 'exit', 'quit', or 'bye'.
        /// </summary>
        internal static void Run(string userName)
        {
            bool running = true;

            while (running)
            {
                // ──  Show user prompt ───────────────────────────────────────────
                Display.ShowUserPrompt(userName);
                string? rawInput = Console.ReadLine();

                // ──  Validate input ─────────────────────────────────────────────
                var (status, errorMessage) = InputValidator.Validate(rawInput);

                if (status != ValidationStatus.Valid)
                {
                    // Invalid input — show friendly error and loop back
                    Display.ShowBotResponse("⚠️  " + errorMessage);
                    continue;
                }

                string input = rawInput!.Trim();

                // ──  Check for built-in commands ────────────────────────────────
                string lower = input.ToLowerInvariant();

                switch (lower)
                {
                    case "exit":
                    case "quit":
                    case "bye":
                    case "goodbye":
                        ShowGoodbye(userName);
                        running = false;
                        continue;

                    case "help":
                        Display.ShowBotResponse(ResponseEngine.GetHelp());
                        continue;

                    case "clear":
                        Console.Clear();
                        Display.ShowLogo();
                        Display.WriteColor(
                            $"  Welcome back, {userName}! Type 'help' to see all topics.\n",
                            ConsoleColor.Green);
                        continue;
                }

                // ── 4. Match against predefined responses ─────────────────────────
                string? response = ResponseEngine.GetResponse(input);

                if (response is not null)
                {
                    Display.ShowBotResponse(response);
                }
                else
                {
                    // ── 5. Graceful fallback for unrecognised input ────────────────
                    Display.ShowBotResponse(
                        "  I didn't quite understand that. Could you rephrase?\n\n" +
                        "    I specialise in cybersecurity topics. Try asking about:\n" +
                        "      phishing | passwords | links | malware | 2FA | privacy\n\n" +
                        "    Or type 'help' to see the full list of available topics.");
                }
            }
        }

        // ─────────────────────────────────────────────────────────────
        //  Goodbye screen
        // ─────────────────────────────────────────────────────────────

        private static void ShowGoodbye(string name)
        {
            Console.WriteLine();
            Display.Border('═', 72, ConsoleColor.Cyan);

            Display.WriteColor($"\n     Goodbye, {name}! Thank you for learning with the Cybersecurity Bot.",
                               ConsoleColor.Green);
            Display.WriteColor("       Stay vigilant, stay informed, and keep South Africa cyber-safe.",
                               ConsoleColor.White);
            Display.WriteColor("\n       🇿🇦  Department of Cybersecurity — Protecting Citizens Online\n",
                               ConsoleColor.Yellow);

            Display.Border('═', 72, ConsoleColor.Cyan);
            Console.WriteLine();

            Thread.Sleep(1800); 
        }
    }
}
