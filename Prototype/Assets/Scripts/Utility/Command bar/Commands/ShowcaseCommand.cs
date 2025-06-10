using UnityEngine.SceneManagement;

public class ShowcaseCommand : IConsoleCommand
{
    public string Name => "showcase";
    public string Description => "Loads the Showcase scene.";

    public void Execute(string[] args)
    {
        SceneManager.LoadScene("Showcase");
    }
}
