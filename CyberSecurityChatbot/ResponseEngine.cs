using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;

namespace CybersecurityChatbot
{
    internal static class ResponseEngine
    {
        // ── Data structure ────────────────────────────────────────────────────────
        // Each entry pairs an array of trigger keywords with a response string.
        // The engine does a case-insensitive substring search.

        private static readonly List<(string[] Keywords, string Response)> Responses = new()
        {
            // ── CONVERSATIONAL — "How are you?" ───────────────────────────────────
            (
                new[] { "how are you", "how r you", "how do you do", "are you ok",
                        "how you doing", "how are u" },
                "    I'm doing great, thank you for asking!\n" +
                "    I'm fully charged and ready to help keep you cyber-safe today.\n\n" +
                "    How can I assist you? You can ask me about phishing, passwords,\n" +
                "    safe browsing, malware, and much more. Type 'help' to see all topics."
            ),
 
            // ── CONVERSATIONAL — "What is your purpose?" ──────────────────────────
            (
                new[] { "purpose", "what are you", "who are you", "what do you do",
                        "what is your goal", "why do you exist", "your purpose" },
                "  MY PURPOSE\n\n" +
                "    I am the Cybersecurity Awareness Bot, created by the\n" +
                "    South African Department of Cybersecurity.\n\n" +
                "    My mission is to:\n" +
                "      • Educate South African citizens on cyber threats\n" +
                "      • Simulate real-life scenarios involving cyber risks\n" +
                "      • Provide practical guidance on staying safe online\n\n" +
                "    South Africa has seen a significant rise in cyber attacks\n" +
                "    targeting individuals, businesses, and government institutions.\n" +
                "    I'm here to help YOU become part of the solution. "
            ),
 
            // ── CONVERSATIONAL — "What can I ask about?" ──────────────────────────
            (
                new[] { "what can i ask", "what can you help", "topics", "what do you know",
                        "what can you do", "capabilities", "what can i learn" },
                "  THINGS YOU CAN ASK ME ABOUT\n\n" +
                "      Phishing        — Fake emails and how to spot them\n" +
                "      Passwords       — Creating strong, secure passwords\n" +
                "      Links / URLs    — Spotting dangerous or fake links\n" +
                "      Malware         — Viruses, ransomware, spyware\n" +
                "      Social Engineering — Manipulation and deception tactics\n" +
                "      Safe Browsing   — Staying safe on the internet\n" +
                "      2FA             — Two-factor authentication explained\n" +
                "      Privacy / POPIA — Your data protection rights in SA\n\n" +
                "    You can type any of these words or ask a full question!"
            ),
 
            // ── CYBERSECURITY — Phishing ──────────────────────────────────────────
            (
                new[] { "phishing", "phish", "fake email", "scam email",
                        "email scam", "fraudulent email" },
                "  PHISHING AWARENESS\n\n" +
                "    Phishing is when cybercriminals send fake emails pretending\n" +
                "    to be trusted organisations (banks, SARS, government) in\n" +
                "    order to steal your personal information.\n\n" +
                "      Warning signs:\n" +
                "      • Urgent language: 'Act NOW or your account will be closed!'\n" +
                "      • Misspelled sender addresses (e.g. support@absa-secure.co)\n" +
                "      • Requests for passwords or ID numbers via email\n" +
                "      • Links that don't match the company's real website\n" +
                "      • Poor grammar and spelling\n\n" +
                "       What to do:\n" +
                "      • Never click links in unexpected emails\n" +
                "      • Type the website address directly into your browser\n" +
                "      • Report phishing to the SA Cybercrime Hub: 0861 000 272\n" +
                "      • Forward suspicious emails to your bank's fraud team"
            ),
 
            // ── CYBERSECURITY — Password safety ───────────────────────────────────
            (
                new[] { "password", "passwords", "strong password",
                        "passphrase", "password safety", "password tips" },
                "  PASSWORD SAFETY\n\n" +
                "    A strong password is your very first line of digital defence.\n\n" +
                "      Good habits:\n" +
                "      • Use at least 12 characters\n" +
                "      • Mix UPPERCASE, lowercase, numbers & symbols (!@#$%)\n" +
                "      • Use a passphrase: e.g. 'Coffee!Rain@Joburg2025'\n" +
                "      • Never reuse the same password on multiple sites\n" +
                "      • Use a reputable password manager (Bitwarden, 1Password)\n" +
                "      • Change passwords immediately if a breach is suspected\n\n" +
                "         Avoid:\n" +
                "      • 'password123', '123456', your name, or your birthday\n" +
                "      • Sharing passwords with anyone — even trusted people\n" +
                "      • Writing passwords on sticky notes near your computer"
            ),
 
            // ── CYBERSECURITY — Suspicious links ──────────────────────────────────
            (
                new[] { "link", "links", "url", "suspicious link",
                        "malicious link", "fake website", "website" },
                "  RECOGNISING SUSPICIOUS LINKS\n\n" +
                "    Cybercriminals disguise dangerous links to look legitimate.\n" +
                "    Always inspect a URL before clicking it.\n\n" +
                "      Red flags in a URL:\n" +
                "      • Misspellings: 'gooogle.com' or 'nedbank-secure.xyz'\n" +
                "      • Unusual extensions: '.ru', '.tk', '.xyz' for local services\n" +
                "      • HTTP instead of HTTPS (no padlock in the browser 🔒)\n" +
                "      • Very long URLs filled with random characters\n" +
                "      • Shortened links (bit.ly, tinyurl) in unsolicited messages\n\n" +
                "      What to do:\n" +
                "      • Hover over a link first to see the real destination URL\n" +
                "      • Use https://www.BeAlert.com to scan suspicious URLs\n" +
                "      • When in doubt — DO NOT click!"
            ),
 
            // ── CYBERSECURITY — Social engineering ────────────────────────────────
            (
                new[] { "social engineering", "manipulation", "pretexting",
                        "baiting", "vishing", "smishing" },
                "   SOCIAL ENGINEERING\n\n" +
                "    Social engineering tricks people into revealing confidential\n" +
                "    information by exploiting trust, fear, or urgency — no hacking needed.\n\n" +
                "    Common tactics:\n" +
                "      • Pretexting  — Fake scenarios ('I'm from IT support')\n" +
                "      • Baiting     — Infected USB drives left in public\n" +
                "      • Vishing     — Fraudulent phone calls pretending to be SARS/banks\n" +
                "      • Smishing    — Fake SMS messages with dangerous links\n" +
                "      • Tailgating  — Following someone into a secure building\n\n" +
                "       Stay safe:\n" +
                "      • Always verify the caller's identity independently\n" +
                "      • Never plug in USB drives you find lying around\n" +
                "      • Legitimate organisations NEVER ask for passwords by phone"
            ),
 
            // ── CYBERSECURITY — Malware ───────────────────────────────────────────
            (
                new[] { "malware", "virus", "ransomware", "spyware",
                        "trojan", "worm", "adware" },
                "  MALWARE PROTECTION\n\n" +
                "    Malware is malicious software designed to harm your device\n" +
                "    or steal your data without your knowledge.\n\n" +
                "    Common types:\n" +
                "      • Virus      — Spreads by attaching itself to files\n" +
                "      • Ransomware — Locks your files and demands payment\n" +
                "      • Spyware    — Secretly monitors your activity\n" +
                "      • Trojan     — Disguises itself as legitimate software\n" +
                "      • Worm       — Self-replicates across networks\n\n" +
                "       Prevention:\n" +
                "      • Keep your OS and software updated at all times\n" +
                "      • Install reputable antivirus software (e.g. Windows Defender)\n" +
                "      • Never download software from untrusted sources\n" +
                "      • Back up important files regularly (rule of 3-2-1 backup)"
            ),
 
            // ── CYBERSECURITY — Safe browsing ─────────────────────────────────────
            (
                new[] { "browsing", "safe browsing", "internet safety",
                        "online safety", "surfing", "web safety" },
                "   SAFE BROWSING TIPS\n\n" +
                "       Best practices:\n" +
                "      • Only use websites with HTTPS (look for 🔒 in the address bar)\n" +
                "      • Avoid banking or shopping on public Wi-Fi networks\n" +
                "      • Use a VPN when connecting to public or unsecured Wi-Fi\n" +
                "      • Enable two-factor authentication (2FA) on all accounts\n" +
                "      • Log out of accounts when done — especially on shared devices\n" +
                "      • Keep your browser and extensions up to date\n" +
                "      • Clear cookies and cache regularly\n\n" +
                "       On mobile:\n" +
                "      • Install apps ONLY from official stores (Google Play / App Store)\n" +
                "      • Check app permissions — a torch app shouldn't need your contacts!"
            ),
 
            // ── CYBERSECURITY — Two-factor authentication ─────────────────────────
            (
                new[] { "two factor", "2fa", "mfa", "multi factor",
                        "authentication", "otp", "one time pin" },
                "   TWO-FACTOR AUTHENTICATION (2FA)\n\n" +
                "    2FA adds an extra layer of security beyond just a password.\n" +
                "    Even if a criminal steals your password, they still can't\n" +
                "    log in without your second factor.\n\n" +
                "    Types of 2FA (strongest → weakest):\n" +
                "      1. Authenticator app (Google Authenticator, Authy)   \n" +
                "      2. Hardware security key (YubiKey)                   \n" +
                "      3. SMS one-time password (OTP)                       \n\n" +
                "       Enable 2FA on your:\n" +
                "      • Email accounts (Gmail, Outlook)\n" +
                "      • Online banking\n" +
                "      • Social media (Facebook, Instagram, X/Twitter)\n" +
                "      • Any account that holds sensitive data"
            ),
 
            // ── CYBERSECURITY — Privacy & POPIA ───────────────────────────────────
            (
                new[] { "privacy", "personal data", "data protection",
                        "popi", "popia", "information regulator" },
                "   PRIVACY & DATA PROTECTION (POPIA)\n\n" +
                "    South Africa's POPIA (Protection of Personal Information Act)\n" +
                "    gives you legal rights over your personal data.\n\n" +
                "    Your rights under POPIA:\n" +
                "      • Know what personal data is being collected about you\n" +
                "      • Request correction of incorrect information\n" +
                "      • Object to the processing of your data\n" +
                "      • Lodge a complaint with the Information Regulator\n\n" +
                "      Protect your privacy:\n" +
                "      • Limit what you share on social media\n" +
                "      • Read privacy policies before signing up for services\n" +
                "      • Use a separate email address for newsletters / sign-ups\n" +
                "      • Report data breaches to: inforeg@justice.gov.za"
            ),
        };

        // ─────────────────────────────────────────────────────────────────────────
        //  Public interface
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Searches predefined responses for any matching keyword.
        /// Returns the response string, or null if no match is found.
        /// </summary>
        internal static string? GetResponse(string userInput)
        {
            string lower = userInput.ToLowerInvariant();

            foreach (var (keywords, response) in Responses)
            {
                foreach (string keyword in keywords)
                {
                    if (lower.Contains(keyword))
                        return response;
                }
            }

            return null; // caller handles the unrecognised-input case
        }

        /// <summary>Returns the full help / topic list.</summary>
        internal static string GetHelp() =>
            "   AVAILABLE TOPICS & COMMANDS\n\n" +
            "    Ask me anything about these topics:\n\n" +
            "       phishing           Fake emails and how to spot them\n" +
            "       password           Strong password best practices\n" +
            "       link / url         Spotting malicious links\n" +
            "       social engineering Manipulation and deception tactics\n" +
            "       malware            Viruses, ransomware, spyware\n" +
            "       browsing           Safe internet habits\n" +
            "       2fa / otp          Two-factor authentication\n" +
            "       privacy / popia    Data protection rights in SA\n\n" +
            "    Conversational:\n" +
            "       how are you        Check on the bot\n" +
            "      purpose            What is this bot for?\n" +
            "      what can i ask     List all available topics\n\n" +
            "    Commands:\n" +
            "    help                   Show this list\n" +
            "    clear                  Clear the screen\n" +
            "    exit / quit / bye      Close the chatbot";
    }
}