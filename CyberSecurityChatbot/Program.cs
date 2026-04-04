using CybersecurityChatbot;

// Step 1: Play the voice greeting WAV on startup
VoiceGreeting.Play();

// Step 2: Display the ASCII logo / title screen
Display.ShowLogo();

// Step 3: Ask for the user's name and show a personalised welcome
string userName = Display.PromptForName();
Display.ShowWelcome(userName);

// Step 4: Enter the main conversation loop
ChatBot.Run(userName);
