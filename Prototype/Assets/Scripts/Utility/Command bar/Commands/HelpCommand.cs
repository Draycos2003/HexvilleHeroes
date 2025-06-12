using System;
using System.Collections.Generic;

public class HelpCommand : IConsoleCommand
{
    private readonly Dictionary<string, IConsoleCommand> _commands;
    private readonly Action<string> _log;

    public HelpCommand(Dictionary<string, IConsoleCommand> commands, Action<string> output)
    {
        _commands = commands;
        _log = output;
    }

    public string Name => "help";
    public string Description => "Lists all commands by category.";
    public string Category => "SYSTEM";

    public void Execute(string[] args)
    {
        var buckets = new Dictionary<string, List<IConsoleCommand>>();
        foreach (var cmd in _commands.Values)
        {
            if (string.IsNullOrEmpty(cmd.Category)) continue;
            if (!buckets.TryGetValue(cmd.Category, out var list))
                buckets[cmd.Category] = list = new List<IConsoleCommand>();
            list.Add(cmd);
        }

        _log($"Commands:");
        foreach (var category in buckets.Keys)
        {
            _log($"{category}:");
            foreach (var cmd in buckets[category])
            {
                _log($"  {cmd.Name} - {cmd.Description}");
            }
            _log("");
        }
    }
}
