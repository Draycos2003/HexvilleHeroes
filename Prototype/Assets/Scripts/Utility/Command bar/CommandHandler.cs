using System;
using System.Collections.Generic;
using UnityEngine;

public class CommandHandler
{
    private Dictionary<string, IConsoleCommand> commands = new();

    public CommandHandler()
    {
        Register(new ShowcaseCommand());
        Register(new HelpCommand(commands));
    }

    public void Register(IConsoleCommand cmd)
    {
        commands[cmd.Name.ToLower()] = cmd;
    }

    public void Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return;

        string[] parts = input.Trim().ToLower().Split(' ');
        string cmdName = parts[0];
        string[] args = parts.Length > 1 ? parts[1..] : Array.Empty<string>();

        if (commands.TryGetValue(cmdName, out var command))
        {
            command.Execute(args);
        }
        else
        {
            Debug.LogWarning("Unknown command: " + cmdName);
        }
    }
}
