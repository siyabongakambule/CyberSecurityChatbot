using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CybersecurityChatbot
{
    internal static class Display
    {
        // ─────────────────────────────────────────────────────────────
        //  Colour helpers
        // ─────────────────────────────────────────────────────────────

        /// <summary>Writes a line (or inline text) in the given colour.</summary>
        internal static void WriteColor(string text,
                                        ConsoleColor color,
                                        bool newLine = true)
        {
            Console.ForegroundColor = color;
            if (newLine)
                Console.WriteLine(text);
            else
                Console.Write(text);
            Console.ResetColor();
        }

        /// <summary>Prints a full-width horizontal border.</summary>
        internal static void Border(char ch = '═',
                                    int width = 72,
                                    ConsoleColor color = ConsoleColor.Cyan)
        {
            WriteColor(new string(ch, width), color);
        }

        // ─────────────────────────────────────────────────────────────
        //  Typing effect — simulates the bot "thinking"
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Prints each character with a small delay to create a
        /// conversational typing feel.
        /// </summary>
        internal static void TypeWrite(string text,
                                       ConsoleColor color = ConsoleColor.White,
                                       int delayMs = 18)
        {
            Console.ForegroundColor = color;
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delayMs);
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        // ─────────────────────────────────────────────────────────────
        //  ASCII logo / title screen
        // ─────────────────────────────────────────────────────────────

        internal static void ShowLogo()
        {
            Console.Clear();
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Top banner
            Border('═', 72, ConsoleColor.Cyan);
            WriteColor(@"
   ██████╗██╗   ██╗██████╗ ███████╗██████╗     ██████╗  ██████╗ ████████╗
  ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗    ██╔══██╗██╔═══██╗╚══██╔══╝
  ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝    ██████╔╝██║   ██║   ██║   
  ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗    ██╔══██╗██║   ██║   ██║   
  ╚██████╗   ██║   ██████╔╝███████╗██║  ██║    ██████╔╝╚██████╔╝   ██║   
   ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝    ╚═════╝  ╚═════╝    ╚═╝  
", ConsoleColor.Cyan);

            // Shield mascot
            WriteColor(@"
              ╔══════════════════════════════════════════════════╗
              ║                                                  ║
              ║      🛡️   CYBERSECURITY AWARENESS BOT  🛡️        ║
              ║       Protecting South African Citizens          ║
              ║         Department of Cybersecurity              ║
              ║                                                  ║
              ╚══════════════════════════════════════════════════╝
", ConsoleColor.Green);

            // ASCII robot mascot
            WriteColor(@"
                         /\_____/\
                        /  o   o  \
                       ( ==  ^  == )      Stay Safe. Stay Smart.
                        )         (       Your shield starts HERE.
                       (  (     )  )
                      ( __(_____)__ )
", ConsoleColor.Yellow);

            Border('═', 72, ConsoleColor.Cyan);
            Console.WriteLine();
        }

        // ─────────────────────────────────────────────────────────────
        //  Name prompt with validation
        // ─────────────────────────────────────────────────────────────

        internal static string PromptForName()
        {
            string name = string.Empty;

            while (string.IsNullOrWhiteSpace(name))
            {
                WriteColor("    Please enter your name to get started: ",
                           ConsoleColor.White, newLine: false);
                Console.ForegroundColor = ConsoleColor.Yellow;
                name = (Console.ReadLine() ?? string.Empty).Trim();
                Console.ResetColor();

                if (string.IsNullOrWhiteSpace(name))
                {
                    WriteColor("\n     Name cannot be empty. Please try again.\n",
                               ConsoleColor.Red);
                }
                else if (name.Length > 50)
                {
                    WriteColor("\n     Name is too long (max 50 characters). Please try again.\n",
                               ConsoleColor.Red);
                    name = string.Empty;
                }
            }

            return name;
        }

        // ─────────────────────────────────────────────────────────────
        //  Welcome screen (shown after name is captured)
        // ─────────────────────────────────────────────────────────────

        internal static void ShowWelcome(string name)
        {
            Console.Clear();
            ShowLogo();

            Border('─', 72, ConsoleColor.DarkCyan);
            Console.WriteLine();
            TypeWrite($"    Welcome, {name}! Great to have you here.",
                      ConsoleColor.Green, delayMs: 22);
            Console.WriteLine();
            TypeWrite("  I am your Cybersecurity Awareness Assistant —",
                      ConsoleColor.White, delayMs: 14);
            TypeWrite("  here to help keep YOU safe online in South Africa.",
                      ConsoleColor.White, delayMs: 14);
            Console.WriteLine();

            WriteColor("    Topics I can help you with:", ConsoleColor.Cyan);
            WriteColor("         Password safety", ConsoleColor.Gray);
            WriteColor("         Phishing & email scams", ConsoleColor.Gray);
            WriteColor("         Suspicious links & websites", ConsoleColor.Gray);
            WriteColor("         Malware & ransomware", ConsoleColor.Gray);
            WriteColor("         Social engineering", ConsoleColor.Gray);
            WriteColor("         Safe browsing habits", ConsoleColor.Gray);
            WriteColor("         Two-factor authentication (2FA)", ConsoleColor.Gray);
            WriteColor("         Privacy & POPIA rights", ConsoleColor.Gray);
            Console.WriteLine();

            WriteColor("    You can also ask me:", ConsoleColor.Cyan);
            WriteColor("       'How are you?'  |  'What is your purpose?'  |  'What can I ask about?'",
                       ConsoleColor.Gray);
            Console.WriteLine();

            Border('─', 72, ConsoleColor.DarkCyan);
            WriteColor("\n  Type 'help' for all commands, or just start chatting!\n",
                       ConsoleColor.Yellow);
            Border('─', 72, ConsoleColor.DarkCyan);
            Console.WriteLine();
        }

        // ─────────────────────────────────────────────────────────────
        //  Reusable prompt / bot label
        // ─────────────────────────────────────────────────────────────

        internal static void ShowUserPrompt(string name)
        {
            Console.WriteLine();
            WriteColor($"  [ {name} ] ➤ ", ConsoleColor.Yellow, newLine: false);
        }

        internal static void ShowBotResponse(string message)
        {
            Console.WriteLine();
            WriteColor("  [ CyberBot  ]", ConsoleColor.Cyan);
            Border('─', 72, ConsoleColor.DarkGray);
            Console.WriteLine();

            // Print each line indented, with a tiny delay per line for feel
            foreach (string line in message.Split('\n'))
            {
                Console.WriteLine("  " + line);
                Thread.Sleep(12);
            }

            Console.WriteLine();
            Border('─', 72, ConsoleColor.DarkGray);
        }
    }
}


