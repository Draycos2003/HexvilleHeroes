using System;
using System.Collections.Generic;

public class HiddenTemplateCommand : IConsoleCommand
{
    // The text you type to invoke this command:
    public string Name => "template";

    // A brief description of what the command does:
    public string Description => "A hidden placeholder command.";

    // Empty category means dont show in help, This will be used to organize the commands into groups/categories:
    public string Category => "";

    private readonly Dictionary<string, IConsoleCommand> _commands;
    private readonly Action<string> _log;

    public HiddenTemplateCommand(
        Dictionary<string, IConsoleCommand> commands,
        Action<string> log
    )
    {
        _commands = commands;
        _log = log;
    }

    public void Execute(string[] args)
    {
        // Intentionally blank. No output, no action. You will put your code of what you want your command to do here.
    }
}
