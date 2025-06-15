using System;
using System.Collections.Generic;

public class CommandHandler
{
    private Dictionary<string, IConsoleCommand> commands = new();
    private Action<string> log;

    // For any of my classmates, this is where you will add (Register) new commands to run scripts or log something, after you make the script for your command.
    public CommandHandler(Action<string> outputLogger)
    {
        log = outputLogger;
        Register(new ShowcaseCommand(log));

        Register(new HelpCommand(commands, log));

        Register(new SceneCommand(log));
    }

    public void Register(IConsoleCommand cmd)
    {
        commands[cmd.Name.ToLower()] = cmd;
    }

    public void Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return;

        string[] parts = input.Trim().Split(' ');
        string cmd = parts[0].ToLower();
        string[] args = parts.Length > 1 ? parts[1..] : Array.Empty<string>();

        if (commands.TryGetValue(cmd, out var command))
        {
            command.Execute(args);
        }
        else
        {
            log($"Unknown command: {cmd}");
        }
    }
}
