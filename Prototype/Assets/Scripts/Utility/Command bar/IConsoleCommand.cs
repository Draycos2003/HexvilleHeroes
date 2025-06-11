public interface IConsoleCommand
{
    string Name { get; }
    string Description { get; }
    string Category { get; }
    void Execute(string[] args);
}
