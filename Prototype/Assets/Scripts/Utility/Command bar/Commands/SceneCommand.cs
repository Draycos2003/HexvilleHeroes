using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SceneCommand : IConsoleCommand
{
    private readonly Action<string> _log;
    public SceneCommand(Action<string> log) => _log = log;

    public string Name => "scene";
    public string Description => "Loads a scene by name or build-index";
    public string Category => "LEVEL";

    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            _log("Usage: scene <buildIndex|sceneName>");
            return;
        }

        var key = args[0];
        _log($"Loading scene “{key}”…");

        // try numeric first, otherwise fall back to name
        if (int.TryParse(key, out var idx))
            SceneManager.LoadScene(idx);
        else
            SceneManager.LoadScene(key);
    }
}