using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HelpCommand : IConsoleCommand
{
    private Dictionary<string, IConsoleCommand> commandMap;

    public HelpCommand(Dictionary<string, IConsoleCommand> commands)
    {
        commandMap = commands;
    }

    public string Name => "help";
    public string Description => "Lists all available commands.";

    public void Execute(string[] args)
    {
        foreach (var cmd in commandMap.Values)
        {
            Debug.Log($"{cmd.Name} - {cmd.Description}");
        }
    }
}
