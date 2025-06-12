using UnityEngine.SceneManagement;
using System;

public class ShowcaseCommand : IConsoleCommand
{
    private readonly Action<string> _log;

    public ShowcaseCommand(Action<string> output)
    {
        _log = output;
    }

    public string Name => "showcase";
    public string Description => "Loads the Showcase scene.";
    public string Category => "INSTRUCTOR";
    public void Execute(string[] args)
    {
        _log("Loading Showcase scene...");
        SceneManager.LoadScene("Showcase");
        SceneManager.LoadScene("Tutorial");
    }
}
