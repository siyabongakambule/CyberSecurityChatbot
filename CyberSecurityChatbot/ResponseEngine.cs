using System;
using System.Collections.Generic;

namespace CybersecurityChatbot
{
    internal static class ResponseEngine
    {
        private static readonly Random Rng = new();

        // ── Random response pools ─────────────────────────────────────────────────
        private static readonly Dictionary<string, List<string>> RandomPools = new()
        {
            ["phishing"] = new List<string>
            {
                "  Be cautious of emails asking for personal information — scammers often disguise themselves as trusted organisations like SARS or your bank.",
                "  Always check the sender's email address carefully. Fake domains like 'support@absa-secure.co' are a classic phishing red flag.",
                "  If an email creates urgency like 'Your account will be closed in 24 hours!' — stop and verify directly with the company before clicking anything.",
                "  Hover over links in emails before clicking. If the real URL looks suspicious or does not match the company name — do not click it!",
                "  Legitimate organisations will NEVER ask for your password, ID number, or banking PIN via email. Ever."
            },
            ["password"] = new List<string>
            {
                "  Use at least 12 characters and mix uppercase, lowercase, numbers and symbols. Example: Coffee!Rain@Joburg2025",
                "  Never reuse the same password on multiple websites. If one site gets hacked, all your accounts become vulnerable.",
                "  Use a password manager like Bitwarden or 1Password to generate and store strong unique passwords for every account.",
                "  Avoid using personal details in passwords — your name, birthday, or ID number are the first things hackers try.",
                "  Change your passwords immediately if you suspect a breach, and always enable two-factor authentication alongside them."
            },
            ["privacy"] = new List<string>
            {
                "  Review your social media privacy settings regularly — limit who can see your posts, location, and personal details.",
                "  Under South Africa's POPIA Act you have the right to know what personal data companies collect about you and request its deletion.",
                "  Use a separate email address for newsletters and sign-ups to protect your main inbox from spam and data breaches.",
                "  Read privacy policies before signing up for apps and services — look for how they share your data with third parties.",
                "  Enable two-factor authentication on all accounts that store personal information — email, banking, and social media."
            },
            ["scam"] = new List<string>
            {
                "  If something sounds too good to be true online — it almost certainly is. Lottery wins, job offers, and prize notifications are common scam triggers.",
                "  Never transfer money to someone you have only met online. Romance scams are among the most financially damaging in South Africa.",
                "  SARS will never ask for your banking details via SMS or WhatsApp. Any such message is a scam — report it immediately.",
                "  Be careful of fake online shops. Always verify the website has HTTPS, check reviews, and use secure payment methods.",
                "  If you receive an unexpected call from your 'bank' asking to verify your card details — hang up and call your bank directly."
            },
            ["malware"] = new List<string>
            {
                "  Never download software from untrusted websites. Stick to official sources like the Microsoft Store or the developer's own site.",
                "  Keep your operating system and antivirus software updated — most malware exploits known vulnerabilities in outdated software.",
                "  Never plug in a USB drive you found lying around — this is a classic malware delivery method called baiting.",
                "  Back up your important files regularly using the 3-2-1 rule: 3 copies, 2 different media types, 1 offsite backup.",
                "  If your computer suddenly becomes very slow or your files become encrypted — disconnect from the internet immediately."
            },
            ["browsing"] = new List<string>
            {
                "  Always look for HTTPS and the padlock icon in your browser before entering any personal or payment information.",
                "  Avoid using public Wi-Fi for banking or shopping. If you must, connect through a VPN first.",
                "  Keep your browser and its extensions updated — outdated browser plugins are a common entry point for attackers.",
                "  Use a browser extension like uBlock Origin to block malicious ads and tracking scripts while you browse.",
                "  Clear your cookies and browser cache regularly, especially on shared or public computers."
            },
            ["2fa"] = new List<string>
            {
                "  Enable two-factor authentication on every important account — email, banking, and social media especially.",
                "  Use an authenticator app like Google Authenticator or Authy instead of SMS-based OTPs — they are far more secure.",
                "  Even if someone steals your password, 2FA stops them from logging in without your second factor.",
                "  Never share your OTP with anyone — not even someone claiming to be from your bank or a tech support agent.",
                "  Hardware security keys like YubiKey offer the strongest form of 2FA protection available for consumers."
            }
        };

        // ── Keyword topic map ─────────────────────────────────────────────────────
        private static readonly Dictionary<string, string> KeywordTopicMap = new()
        {
            { "phishing", "phishing" },  { "phish", "phishing" },
            { "fake email", "phishing" },{ "scam email", "phishing" },
            { "email scam", "phishing" },{ "suspicious email", "phishing" },
            { "social engineering", "phishing" }, { "vishing", "phishing" },
            { "smishing", "phishing" },
            { "password", "password" },  { "passwords", "password" },
            { "passphrase", "password" },{ "strong password", "password" },
            { "privacy", "privacy" },    { "popia", "privacy" },
            { "personal data", "privacy" }, { "data protection", "privacy" },
            { "scam", "scam" },          { "fraud", "scam" },
            { "malware", "malware" },    { "virus", "malware" },
            { "ransomware", "malware" }, { "spyware", "malware" },
            { "trojan", "malware" },     { "worm", "malware" },
            { "browsing", "browsing" },  { "safe browsing", "browsing" },
            { "internet safety", "browsing" }, { "online safety", "browsing" },
            { "wifi", "browsing" },      { "wi-fi", "browsing" },
            { "link", "browsing" },      { "url", "browsing" },
            { "2fa", "2fa" },            { "two factor", "2fa" },
            { "otp", "2fa" },            { "authentication", "2fa" },
            { "mfa", "2fa" },            { "one time pin", "2fa" }
        };

        // ── Conversational responses ──────────────────────────────────────────────
        private static readonly Dictionary<string, string> ConversationalResponses = new()
        {
            ["how are you"] =
                "  I am doing great, thank you for asking!\n" +
                "I am ready to help keep you cyber-safe today.\n" +
                "What cybersecurity topic would you like to explore?",
            ["purpose"] =
                "  I am the Cybersecurity Awareness Bot, created by the\n" +
                "South African Department of Cybersecurity.\n\n" +
                "My mission is to educate South African citizens on cyber threats,\n" +
                "simulate real-life scenarios involving cyber risks, and provide\n" +
                "practical guidance on staying safe online. ",
            ["what can i ask"] =
                "  You can ask me about:\n\n" +
                "  Phishing — fake emails and scam detection\n" +
                "  Passwords — strong password practices\n" +
                "  Privacy / POPIA — data protection rights\n" +
                "  Scams — recognising and avoiding scams\n" +
                "  Malware — viruses, ransomware, spyware\n" +
                "  Browsing — safe internet habits\n" +
                "  2FA — two-factor authentication\n\n" +
                "Just type any topic or ask a full question!",
            ["hello"] =
                "  Hello! Welcome to the Cybersecurity Awareness Bot.\n" +
                "I am here to help you stay safe online in South Africa.\n" +
                "Type 'help' to see all available topics!",
            ["help"] =
                "  AVAILABLE TOPICS\n\n" +
                "  phishing       Fake emails and scams\n" +
                "  password       Strong password practices\n" +
                "  privacy        Data protection and POPIA\n" +
                "  scam           Recognising online scams\n" +
                "  malware        Viruses and ransomware\n" +
                "  browsing       Safe internet habits\n" +
                "  2fa            Two-factor authentication\n\n" +
                "You can also say:\n" +
                "'give me a tip'  |  'tell me more'  |  'another tip'"
        };

        // ── Public methods ────────────────────────────────────────────────────────

        internal static string? DetectTopic(string input)
        {
            string lower = input.ToLowerInvariant();
            foreach (var kvp in KeywordTopicMap)
                if (lower.Contains(kvp.Key)) return kvp.Value;
            return null;
        }

        internal static string GetRandomResponse(string topicKey)
        {
            if (RandomPools.TryGetValue(topicKey, out var pool))
                return pool[Rng.Next(pool.Count)];
            return GetFallback();
        }

        internal static string? GetConversationalResponse(string input)
        {
            string lower = input.ToLowerInvariant();
            if (lower.Contains("how are you")) return ConversationalResponses["how are you"];
            if (lower.Contains("purpose") || lower.Contains("who are you") || lower.Contains("what are you"))
                return ConversationalResponses["purpose"];
            if (lower.Contains("what can i ask") || lower.Contains("what can you"))
                return ConversationalResponses["what can i ask"];
            if (lower == "hello" || lower == "hi" || lower == "hey" || lower.StartsWith("hi "))
                return ConversationalResponses["hello"];
            if (lower == "help" || lower.Contains("show topics"))
                return ConversationalResponses["help"];
            return null;
        }

        internal static string GetFallback() =>
            "  I am not sure I understand that. Could you try rephrasing?\n\n" +
            "I specialise in cybersecurity topics. Try asking about:\n" +
            "phishing | passwords | privacy | scams | malware | browsing | 2FA\n\n" +
            "Or type 'help' to see all available topics.";

        internal static string GetTipForTopic(string topicKey) => GetRandomResponse(topicKey);

        internal static string GetTopicDisplayName(string topicKey) => topicKey switch
        {
            "phishing" => "Phishing & Email Scams",
            "password" => "Password Safety",
            "privacy" => "Privacy & POPIA",
            "scam" => "Online Scams",
            "malware" => "Malware Protection",
            "browsing" => "Safe Browsing",
            "2fa" => "Two-Factor Authentication",
            _ => topicKey
        };

        internal static string GetHelp() =>
            "  AVAILABLE TOPICS AND COMMANDS\n\n" +
            "  phishing           Fake emails and how to spot them\n" +
            "  password           Strong password best practices\n" +
            "  privacy / popia    Data protection rights in SA\n" +
            "  scam               Recognising online scams\n" +
            "  malware            Viruses, ransomware, spyware\n" +
            "  browsing           Safe internet habits\n" +
            "  2fa / otp          Two-factor authentication\n\n" +
            "Conversational:\n" +
            "  how are you        Check on the bot\n" +
            "  purpose            What is this bot for\n" +
            "  what can i ask     List all available topics\n\n" +
            "Quick phrases:\n" +
            "give me another tip   Get another tip on the last topic\n" +
            "tell me more          Continue on the current topic";

        internal static bool IsValidTopic(string topicKey) =>
            RandomPools.ContainsKey(topicKey);
    }
}